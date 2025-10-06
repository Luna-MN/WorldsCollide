shader_type canvas_item;

// single texture coming from your SubViewport
uniform sampler2D viewport_texture;

// how many columns / rows are in the grid
uniform vec2 grid_size = vec2(2.0, 2.0);

// which cell (x,y) to display
uniform vec2 cell_index = vec2(0.0, 0.0);

// pixel size of the SubViewport texture
uniform vec2 viewport_size = vec2(64.0, 64.0);

// flip vertically if your SubViewport appears upside-down
uniform bool flip_v = false;

void fragment()
{
    vec2 cell_uv_size = 1.0 / grid_size;
    vec2 uv = UV;
    if (flip_v) {
        uv.y = 1.0 - uv.y;
    }

    // Map UVs to the correct grid cell
    vec2 new_uv = cell_index * cell_uv_size + uv * cell_uv_size;

    // Snap UVs to pixel centers to avoid sub-pixel blur
    vec2 texel = 1.0 / viewport_size;
    new_uv = (floor(new_uv / texel) + 0.5) * texel;

    COLOR = texture(viewport_texture, new_uv);
}
