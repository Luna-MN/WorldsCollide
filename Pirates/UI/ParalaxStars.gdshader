shader_type canvas_item;

uniform float star_density : hint_range(10.0, 200.0) = 80.0;
uniform vec2 parallax_scale = vec2(0.01, 0.01);
uniform vec4 star_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float star_size = 0.02;
uniform float star_brightness = 1.0;

float hash(vec2 p) {
    return fract(sin(dot(p, vec2(12.9898,78.233))) * 43758.5453123);
}

void fragment() {
    // Base texture (so the panel retains its look)
    vec4 base_col = texture(TEXTURE, UV);

    // Create moving UV based on SCREEN_UV (so movement affects stars)
    vec2 star_uv = (SCREEN_UV / parallax_scale);

    // Grid for random star placement
    vec2 grid = floor(star_uv * star_density);
    vec2 f = fract(star_uv * star_density);

    // Random seed per grid cell
    float r = hash(grid);
    float d = length(f - vec2(0.5));

    // Make a star if distance < threshold (small dots)
    float star = smoothstep(star_size, star_size * 0.5, d) * step(0.995, r);

    // Add the stars on top of base color
    vec3 final_col = base_col.rgb + star * star_color.rgb * star_brightness;
    COLOR = vec4(final_col, base_col.a);
}
