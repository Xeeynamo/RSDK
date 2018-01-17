#pragma once
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

typedef unsigned char byte;

struct AnimationFileList
{
	char fileName[32];
	int numAnimations;
	int aniListOffset;
	int cbListOffset;
};
struct FunctionScript
{
	int mainScript;
	int mainJumpTable;
};
struct ObjectScript
{
	int numFrames;
	byte surfaceNum;
	int mainScript;
	int playerScript;
	int drawScript;
	int startupScript;
	int mainJumpTable;
	int playerJumpTable;
	int drawJumpTable;
	int startupJumpTable;
	int frameListOffset;
	AnimationFileList animationFile;
};
class AnimationSystem {
public:
	static AnimationFileList GetDefaultAnimationRef();
};
struct ScriptEngine
{
	int operands[10];
	int tempValue[8];
	int arrayPosition[3];
	int checkResult;
	int sRegister;
};
struct StateScriptEngine {
	int scriptCodePtr;
	int jumpTablePtr;
	int scriptSub;
	int deep;
	bool isSwitchEnd;
	bool error;

	StateScriptEngine& IncDeep() {
		deep++;
		return *this;
	}
};
class ObjectSystem {
	static const byte scriptOpcodeSizes[];

	FILE *m_File;
	int m_stageVarsIndex;

	char functionNames[0x200][32];
	char typeNames[0x100][32];
	char sourceNames[0x100][32];
	char globalVariableNames[0x100][32];
	int globalVariables[0x100];

	int numGlobalSfx;
	char sfxNames[0x100][32];

	int scriptDataPos;
	int jumpTableDataPos;
	int jumpTableStackPos;
	int functionStackPos;
	int scriptData[0x40000];
	int jumpTableData[0x4000];
	int jumpTableStack[0x400];
	int functionStack[0x400];
	int scriptFramesNo;

	ScriptEngine scriptEng;
	ObjectScript objectScriptList[0x100];
	FunctionScript functionScriptList[0x200];

	int NUM_FUNCTIONS;
	int NO_GLOBALVARIABLES;

	int Read32();
	int Read16();
	int Read8();

	void DecompileStatement(FILE* f, StateScriptEngine& state);
public:
	ObjectSystem();
	~ObjectSystem();

	void ClearScriptData();
	void SetObjectTypeName(const char *typeName, int scriptNum);
	void SetObjectSourceName(const char *typeName, int scriptNum);
	void LoadByteCodeFile(const char *filename, int scriptNum);
	int LoadGameConfig(const char *filename);
	void LoadStageFile(const char *stageconfig, const char *script);

	void Decompile();
	void DecompileScript(FILE* f, int scriptCodePtr, int jumpTablePtr, int scriptSub);
};