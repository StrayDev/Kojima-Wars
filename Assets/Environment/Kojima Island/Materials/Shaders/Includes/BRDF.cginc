// Normal distribution function
float trowbridge_reitz_ggx(const float n_dot_h, const float roughness)
{
    const float a = roughness * roughness;
    const float a2 = a * a;
    const float n_dot_h2 = n_dot_h * n_dot_h;

    const float num = a2;
    float de_nom = (n_dot_h2 * (a2 - 1.0h) + 1.0h);
    de_nom = PI * de_nom * de_nom;

    return num / de_nom;
}

float geometry_schlick_ggx(const float n_dot_v, const float roughness)
{
    const float r = roughness + 1.0h; // Disney modification to remap roughness to reduce roughness 'Hotness'
    const float k = (r * r) / 8.0h;

    const float num = n_dot_v;
    const float de_nom = n_dot_v * (1.0h - k) + k;

    return num / de_nom;
}

// Geometry function
float geometry_smith(const float n_dot_v, const float n_dot_l, const float roughness)
{
    const float ggx1 = geometry_schlick_ggx(n_dot_v, roughness);
    const float ggx2 = geometry_schlick_ggx(n_dot_l, roughness);

    return ggx1 * ggx2;
}

// Fresnel function
float3 fresnel_schlick(const float n_dot_l, const float3 f0, const float f90)
{
    return f0 + (f90 - f0) * pow(1.0h - n_dot_l, 5.0h);
}

float3 evaluateBRDF(float3 worldPosition, float3 lightDir, float3 normal, float specular, float roughness)
{
    // Get view direction | float vector | reflection direction
    const float3 view_dir		= worldPosition;
    const float3 float_vector		= normalize(view_dir + lightDir);

    const float n_dot_l = saturate(dot(normal, lightDir));		// Dot product of normal and light
    const float n_dot_h = saturate(dot(normal, float_vector));	// Dot product of normal and float vector
    const float n_dot_v = saturate(dot(normal, view_dir));		// Dot product of normal and view
    const float v_dot_h = saturate(dot(view_dir, float_vector));
    
    // Functions for Cook-Torrance BRDF
    float n = trowbridge_reitz_ggx(n_dot_h, roughness);
    float g = geometry_smith(n_dot_v, n_dot_l, roughness) * geometry_smith(n_dot_v, n_dot_l, roughness);
    float3 f = fresnel_schlick(v_dot_h, float3(0.04h, 0.04h, 0.04h), 1.0h);

    return (n * g * f) / (4.0f * n_dot_v * n_dot_l + 0.0001h) * specular;
}