#pragma once

const char opcodeList[][0x20] =
{
	"End",
	"Equal",
	"Add",
	"Sub",
	"Inc",
	"Dec",
	"Mul",
	"Div",
	"ShR",
	"ShL",
	"And",
	"Or",
	"Xor",
	"Mod",
	"FlipSign",
	"CheckEqual",
	"CheckGreater",					// 0x10
	"CheckLower",
	"CheckNotEqual",
	"IfEqual",						// 0x13 19
	"IfGreater",
	"IfGreaterOrEqual",
	"IfLower",
	"IfLowerOrEqual",
	"IfNotEqual",					// 0x18
	"else",							// 0x19 25
	"endif",						// 0x1A
	"WEqual",
	"WGreater",
	"WGreaterOrEqual",
	"WLower",
	"WLowerOrEqual",
	"WNotEqual",					// 0x20
	"loop",							// 0x21
	"switch",						// 0x22
	"break",						// 0x23
	"endswitch",
	"Rand",							// 0x25
	"Sin",
	"Cos",
	"Sin256",
	"Cos256",
	"SinChange",					// 0x2A
	"CosChange",
	"ATan2",
	"Interpolate",
	"InterpolateXY",
	"LoadSpriteSheet",				// 0x2F
	"RemoveSpriteSheet",			// 0x30
	"DrawSprite",
	"DrawSpriteXY",
	"DrawSpriteScreenXY",
	"DrawTintRect",
	"DrawNumbers",
	"DrawActName",
	"DrawMenu",
	"SpriteFrame",
	"SetDebugIcon",
	"LoadPalette",
	"RotatePalette",
	"SetScreenFade",
	"SetActivePalette",
	"SetPaletteFade",
	"CopyPalette",
	"ClearScreen",
	"DrawSpriteFX",
	"DrawSpriteScreenFX",
	"LoadAnimation",				// 0x43
	"SetupMenu",
	"AddMenuEntry",
	"EditMenuEntry",
	"LoadStage",
	"DrawRect",
	"ResetObjectEntity",
	"PlayerObjectCollision",
	"CreateTempObject",
	"BindPlayerToObject",
	"PlayerTileCollision",
	"ProcessPlayerControl",
	"ProcessAnimation",
	"DrawObjectAnimation",
	"DrawPlayerAnimation",
	"SetMusicTrack",				// 0x52
	"PlayMusic",
	"StopMusic",
	"PlaySfx",						// 0x55 85
	"StopSfx",						// 0x56 86
	"SetSfxAttributes",
	"ObjectTileCollision",
	"ObjectTileGrip",
	"LoadVideo",
	"NextVideoFrame",				// 0x5B
	"PlayStageSfx",
	"StopStageSfx",
	"Not",
	"Draw3DScene",
	"SetIdentityMatrix",			// 0x60
	"MatrixMultiply",
	"MatrixTranslateXYZ",
	"MatrixScaleXYZ",				// 0x63
	"MatrixRotateX",
	"MatrixRotateY",
	"MatrixRotateZ",
	"MatrixRotateXYZ",
	"TransformVertices",
	"CallFunction",					// 0x69 105
	"EndFunction",					// 0x6A 106
	"SetLayerDeformation",
	"CheckTouchRect",
	"GetTileLayerEntry",
	"SetTileLayerEntry",
	"GetBit",
	"SetBit",
	"PauseMusic",
	"ResumeMusic",
	"ClearDrawList",
	"AddDrawListEntityRef",
	"GetDrawListEntityRef",
	"SetDrawListEntityRef",
	"Get16x16TileInfo",
	"Copy16x16Tile",
	"Set16x16TileInfo",
	"GetAnimationByName",
	"ReadSaveRAM",
	"WriteSaveRAM",
	"LoadTextFont",
	"LoadTextFile",
	"DrawText",
	"GetTextInfo",
	"GetVersionNumber",
	"SetAchievement",
	"SetLeaderboard",
	"LoadOnlineMenu",
	"EngineCallback"
};