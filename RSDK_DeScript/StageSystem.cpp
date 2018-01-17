#include "ObjectSystem.h"

void ObjectSystem::LoadStageFile(const char *stageconfig, const char *script) {
	FILE *f = fopen(stageconfig, "rb");
	if (f == NULL) return;

	int scriptNum = 1;
	byte flag = fgetc(f);
	if (flag == 1)
		scriptNum += LoadGameConfig("Data/Game/GameConfig.bin");
	m_stageVarsIndex = scriptNum;
	for (int i = 96; i < 128; i++) {
		// Read palette
		fgetc(f); fgetc(f); fgetc(f);
	}
	LoadByteCodeFile(script, scriptNum);

	int objectsCount = fgetc(f);
	for (int i = 0; i < objectsCount; i++) {
		char strBuffer[0x40];
		byte strLen = fgetc(f);
		for (int i = 0; i < strLen; i++)
			strBuffer[i] = fgetc(f);
		strBuffer[strLen] = '\0';
		SetObjectTypeName(strBuffer, i + scriptNum);
	}
	for (int i = 0; i < objectsCount; i++) {
		char strBuffer[0x40];
		byte strLen = fgetc(f);
		for (int i = 0; i < strLen; i++)
			strBuffer[i] = fgetc(f);
		strBuffer[strLen] = '\0';
		SetObjectSourceName(strBuffer, i + scriptNum);
	}

	// Read sound effects
	int sfxCount = fgetc(f);
	for (int i = 0; i < sfxCount; i++) {
		int strLen = fgetc(f);
		for (int j = 0; j < strLen; j++)
			sfxNames[numGlobalSfx + i][j] = fgetc(f);
		sfxNames[numGlobalSfx + i][strLen] = '\0';
	}
}