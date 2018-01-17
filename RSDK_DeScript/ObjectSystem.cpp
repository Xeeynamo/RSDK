#include "ObjectSystem.h"


int ObjectSystem::Read32() {
	return fgetc(m_File) + (fgetc(m_File) << 8) + (fgetc(m_File) << 16) + (fgetc(m_File) << 24);
}
int ObjectSystem::Read16() {
	return fgetc(m_File) + (fgetc(m_File) << 8);
}
int ObjectSystem::Read8() {
	return fgetc(m_File);
}

ObjectSystem::ObjectSystem()
{
	ClearScriptData();
}
ObjectSystem::~ObjectSystem() {

}

void ObjectSystem::ClearScriptData() {
	for (int i = 0; i < 0x40000; i++)
		scriptData[i] = 0;
	for (int i = 0; i < 0x4000; i++)
		jumpTableData[i] = 0;
	scriptDataPos = 0;
	jumpTableDataPos = 0;
	scriptFramesNo = 0;
	NUM_FUNCTIONS = 0;
	//AnimationSystem.ClearAnimationData();
	for (int i = 0; i < 2; i++)
	{
		//PlayerSystem.playerList[i].animationFile = AnimationSystem.GetDefaultAnimationRef();
		//PlayerSystem.playerList[i].objectPtr = this->objectEntityList[0];
	}
	for (int i = 0; i < 256; i++)
	{
		objectScriptList[i].mainScript = 0x3FFFF;
		objectScriptList[i].mainJumpTable = 0x3FFF;
		objectScriptList[i].playerScript = 0x3FFFF;
		objectScriptList[i].playerJumpTable = 0x3FFF;
		objectScriptList[i].drawScript = 0x3FFFF;
		objectScriptList[i].drawJumpTable = 0x3FFF;
		objectScriptList[i].startupScript = 0x3FFFF;
		objectScriptList[i].startupJumpTable = 0x3FFF;
		objectScriptList[i].frameListOffset = 0;
		objectScriptList[i].numFrames = 0;
		objectScriptList[i].surfaceNum = 0;
		objectScriptList[i].animationFile = AnimationSystem::GetDefaultAnimationRef();
		functionScriptList[i].mainScript = 0x3FFFF;
		functionScriptList[i].mainJumpTable = 0x3FFF;
		typeNames[i][0] = '\0';
	}

	numGlobalSfx = 0;
	for (int i = 0; i < 0x100; i++)
		sfxNames[i][0] = '\0';

	SetObjectTypeName("BlankObject", 0);
}
void ObjectSystem::SetObjectTypeName(const char *typeName, int scriptNum) {
	int count = 0;
	int length = strlen(typeName);
	for (int i = 0; i < length; i++)
	{
		if (typeName[i] != '\0')
		{
			if (typeName[i] != ' ')
				typeNames[scriptNum][count++] = typeName[i];
		}
		else
			break;
	}
	if (count >= sizeof(*typeNames))
		return;
	typeNames[scriptNum][count] = '\0';
}
void ObjectSystem::SetObjectSourceName(const char *typeName, int scriptNum) {
	int count = 0;
	int length = strlen(typeName);
	for (int i = 0; i < length; i++)
	{
		if (typeName[i] != '\0')
		{
			if (typeName[i] != ' ')
				sourceNames[scriptNum][count++] = typeName[i];
		}
		else
			break;
	}
	if (count >= sizeof(*sourceNames))
		return;
	sourceNames[scriptNum][count] = '\0';
}
void ObjectSystem::LoadByteCodeFile(const char *filename, int scriptNum) {
	m_File = fopen(filename, "rb");
	for (int opcount = Read32(); opcount > 0;)
	{
		byte data = Read8();
		int blocksCount = data & 0x7F;
		if ((data & 0x80) == 0)
		{
			for (int i = 0; i < blocksCount; i++)
				scriptData[scriptDataPos++] = Read8();
			opcount -= blocksCount;
		}
		else
		{
			for (int i = 0; i < blocksCount; i++)
				scriptData[scriptDataPos++] = Read32();
			opcount -= blocksCount;
		}
	}

	for (int opcount = Read32(); opcount > 0; )
	{
		byte data = Read8();
		int blocksCount = data & 0x7F;
		if ((data & 0x80) == 0)
		{
			for (int i = 0; i < blocksCount; i++)
				jumpTableData[jumpTableDataPos++] = Read8();
			opcount -= blocksCount;
		}
		else
		{
			for (int i = 0; i < blocksCount; i++)
				jumpTableData[jumpTableDataPos++] = Read32();
			opcount -= blocksCount;
		}
	}


	int count = Read16();
	for (int i = 0; i < count; i++)
	{
		objectScriptList[scriptNum + i].mainScript = Read32();
		objectScriptList[scriptNum + i].playerScript = Read32();
		objectScriptList[scriptNum + i].drawScript = Read32();
		objectScriptList[scriptNum + i].startupScript = Read32();
	}
	for (int i = 0; i < count; i++)
	{
		objectScriptList[scriptNum + i].mainJumpTable = Read32();
		objectScriptList[scriptNum + i].playerJumpTable = Read32();
		objectScriptList[scriptNum + i].drawJumpTable = Read32();
		objectScriptList[scriptNum + i].startupJumpTable = Read32();
	}

	count = Read16();
	for (int i = 0; i < count; i++)
		functionScriptList[i].mainScript = Read32();
	for (int i = 0; i < count; i++)
		functionScriptList[i].mainJumpTable = Read32();
	fclose(m_File);
	m_File = 0;
}