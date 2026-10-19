"""Generates the HashVerifier logo.

Outputs:
  src/HashVerifier.App/Resources/app.ico  Windows icon (16-256 px)
  docs/images/logo.svg                    Vector logo for the README
  docs/images/logo.png                    256 px PNG

Requires Pillow:  pip install pillow
Usage:            python scripts/generate-icon.py
"""
from pathlib import Path

from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parent.parent

# Geometry, in a 256x256 design grid.
GRID = 256
BG_RECT, BG_RADIUS = (16, 16, 240, 240), 52
GRADIENT = ("#4F46E5", "#06B6D4")  # indigo -> cyan, top-left to bottom-right
HASH_WIDTH = 22
HASH_LINES = [  # the "#" glyph, slightly slanted like a typeset hash
    ((104, 50), (86, 186)),
    ((156, 50), (138, 186)),
    ((50, 94), (194, 94)),
    ((42, 142), (186, 142)),
]
BADGE_CENTER, BADGE_RADIUS, BADGE_RING = (188, 188), 48, 10
BADGE_COLOR = "#22C55E"
CHECK = [(165, 189), (181, 205), (211, 172)]
CHECK_WIDTH = 13
ICO_SIZES = [16, 20, 24, 32, 40, 48, 64, 128, 256]


def hex_rgb(color: str) -> tuple[int, int, int]:
    return tuple(int(color[i:i + 2], 16) for i in (1, 3, 5))


def thick_line(draw: ImageDraw.ImageDraw, points, width: float, fill, scale: float) -> None:
    """Polyline with round caps and joins."""
    pts = [(x * scale, y * scale) for x, y in points]
    w = width * scale
    draw.line(pts, fill=fill, width=round(w), joint="curve")
    for x, y in pts:
        draw.ellipse((x - w / 2, y - w / 2, x + w / 2, y + w / 2), fill=fill)


def render(size: int) -> Image.Image:
    """Draws the logo supersampled 4x and downsizes it for smooth edges."""
    canvas = size * 4
    s = canvas / GRID

    # Diagonal gradient, masked by the rounded square.
    start, end = hex_rgb(GRADIENT[0]), hex_rgb(GRADIENT[1])
    gradient = Image.new("RGB", (canvas, canvas))
    gradient.putdata([
        tuple(round(a + (b - a) * ((x + y) / (2 * canvas))) for a, b in zip(start, end))
        for y in range(canvas) for x in range(canvas)
    ])
    mask = Image.new("L", (canvas, canvas), 0)
    ImageDraw.Draw(mask).rounded_rectangle([c * s for c in BG_RECT], radius=BG_RADIUS * s, fill=255)
    image = Image.new("RGBA", (canvas, canvas), (0, 0, 0, 0))
    image.paste(gradient, mask=mask)

    draw = ImageDraw.Draw(image)
    for line in HASH_LINES:
        thick_line(draw, line, HASH_WIDTH, "white", s)

    cx, cy = BADGE_CENTER
    outer, inner = BADGE_RADIUS + BADGE_RING / 2, BADGE_RADIUS - BADGE_RING / 2
    draw.ellipse([(cx - outer) * s, (cy - outer) * s, (cx + outer) * s, (cy + outer) * s], fill="white")
    draw.ellipse([(cx - inner) * s, (cy - inner) * s, (cx + inner) * s, (cy + inner) * s], fill=BADGE_COLOR)
    thick_line(draw, CHECK, CHECK_WIDTH, "white", s)

    return image.resize((size, size), Image.LANCZOS)


def svg() -> str:
    def poly(points):
        return " ".join(f"{x},{y}" for x, y in points)

    hash_lines = "\n    ".join(
        f'<line x1="{a[0]}" y1="{a[1]}" x2="{b[0]}" y2="{b[1]}"/>' for a, b in HASH_LINES)
    x0, y0, x1, y1 = BG_RECT
    cx, cy = BADGE_CENTER
    return f"""<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 {GRID} {GRID}" width="{GRID}" height="{GRID}" role="img" aria-label="HashVerifier logo">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="1" y2="1">
      <stop offset="0" stop-color="{GRADIENT[0]}"/>
      <stop offset="1" stop-color="{GRADIENT[1]}"/>
    </linearGradient>
  </defs>
  <rect x="{x0}" y="{y0}" width="{x1 - x0}" height="{y1 - y0}" rx="{BG_RADIUS}" fill="url(#bg)"/>
  <g stroke="#fff" stroke-width="{HASH_WIDTH}" stroke-linecap="round">
    {hash_lines}
  </g>
  <circle cx="{cx}" cy="{cy}" r="{BADGE_RADIUS}" fill="{BADGE_COLOR}" stroke="#fff" stroke-width="{BADGE_RING}"/>
  <polyline points="{poly(CHECK)}" fill="none" stroke="#fff" stroke-width="{CHECK_WIDTH}" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
"""


def main() -> None:
    images = {size: render(size) for size in ICO_SIZES}

    ico = ROOT / "src/HashVerifier.App/Resources/app.ico"
    images[256].save(ico, format="ICO", sizes=[(s, s) for s in ICO_SIZES],
                     append_images=[images[s] for s in ICO_SIZES if s != 256])

    docs = ROOT / "docs/images"
    images[256].save(docs / "logo.png")
    (docs / "logo.svg").write_text(svg(), encoding="utf-8")

    print(f"Wrote {ico.relative_to(ROOT)}, docs/images/logo.png, docs/images/logo.svg")


if __name__ == "__main__":
    main()
