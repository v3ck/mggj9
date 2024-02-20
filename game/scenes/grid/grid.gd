extends Node2D

class_name Grid

func TileToPixel(tile_x: int, tile_y: int):
	var pixel_x = 12 * (6 - tile_y) + 24 * tile_x
	var pixel_y = 14 + 21 * tile_y
	return Vector2(pixel_x, pixel_y)
