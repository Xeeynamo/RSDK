#include "ObjectSystem.h"

int ObjectSystem::LoadGameConfig(const char *filename) {
	byte strLen;
	char strB[0x100];
	FILE *f = fopen(filename, "rb");

	// Read game name
	strLen = fgetc(f);
	for (int j = 0; j < strLen; j++)
		strB[j] = fgetc(f);
	strB[strLen] = '\0';

	// Read rsdk data name
	strLen = fgetc(f);
	for (int j = 0; j < strLen; j++)
		strB[j] = fgetc(f);
	strB[strLen] = '\0';

	// Read game description
	strLen = fgetc(f);
	for (int j = 0; j < strLen; j++)
		strB[j] = fgetc(f);
	strB[strLen] = '\0';

	// Read common object names
	byte commonObjectsCount = fgetc(f);
	for (int i = 0; i < commonObjectsCount; i++)
	{
		strLen = fgetc(f);
		int acutalLenght = 0;
		for (int j = 0; j < strLen; j++) {
			char c = fgetc(f);
			if (c != ' ')
				strB[acutalLenght++] = c;
		}
		strB[acutalLenght] = '\0';
		SetObjectTypeName(strB, 1 + i);
	}
	// Read common objects sourcecode
	for (int i = 0; i < commonObjectsCount; i++)
	{
		strLen = fgetc(f);
		for (int j = 0; j < strLen; j++)
			strB[j] = fgetc(f);
		strB[strLen] = '\0';
		SetObjectSourceName(strB, 1 + i);
	}

	// Read global variables
	byte globalVariablesCount = fgetc(f);
	NO_GLOBALVARIABLES = globalVariablesCount;
	for (int i = 0; i < globalVariablesCount; ++i)
	{
		// Read variable name
		strLen = fgetc(f);
		for (int j = 0; j < strLen; j++)
			strB[j] = fgetc(f);
		strB[strLen] = '\0';
		// Store variable name
		strcpy(globalVariableNames[i], strB);
		// Store default value
		globalVariables[i] = (fgetc(f) << 24) | (fgetc(f) << 16) | (fgetc(f) << 8) | fgetc(f);
	}

	// Read sound effect's names variables
	byte sfxCount = fgetc(f);
	numGlobalSfx = sfxCount;
	for (int i = 0; i < sfxCount; ++i)
	{
		strLen = fgetc(f);
		for (int j = 0; j < strLen; j++)
			sfxNames[i][j] = fgetc(f);
		sfxNames[i][strLen] = '\0';
	}

	// Read playable character names
	byte charactersCount = fgetc(f);
	for (int i = 0; i < charactersCount; ++i)
	{
		strLen = fgetc(f);
		for (int j = 0; j < strLen; j++)
			strB[j] = fgetc(f);
		strB[strLen] = '\0';
	}

	// Read presentation stages
	/*byte dummy;
	FileIO.noPresentationStages = FileIO.ReadByte();
	for (int i = 0; i < FileIO.noPresentationStages; ++i)
	{
		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; j++)
			FileIO.pStageList[i].stageFolderName[j] = (char)FileIO.ReadByte();
		FileIO.pStageList[i].stageFolderName[strLen] = '\0';

		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; ++j)
			FileIO.pStageList[i].actNumber[j] = (char)FileIO.ReadByte();
		FileIO.pStageList[i].actNumber[strLen] = '\0';

		byte count = FileIO.ReadByte();
		for (int j = 0; j < count; ++j)
			FileIO.ReadByte();
		dummy = FileIO.ReadByte();
	}

	// Read zone stages
	FileIO.noZoneStages = fgetc(f);
	for (int i = 0; i < FileIO.noZoneStages; ++i)
	{
		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; j++)
			FileIO.zStageList[i].stageFolderName[j] = (char)FileIO.ReadByte();
		FileIO.zStageList[i].stageFolderName[strLen] = '\0';

		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; ++j)
			FileIO.zStageList[i].actNumber[j] = (char)FileIO.ReadByte();
		FileIO.zStageList[i].actNumber[strLen] = '\0';

		byte count = FileIO.ReadByte();
		for (int j = 0; j < count; ++j)
			FileIO.ReadByte();
		dummy = FileIO.ReadByte();
	}

	// Read special stages
	FileIO.noSpecialStages = fgetc(f);
	for (int i = 0; i < FileIO.noSpecialStages; ++i)
	{
		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; j++)
			FileIO.sStageList[i].stageFolderName[j] = (char)FileIO.ReadByte();
		FileIO.sStageList[i].stageFolderName[strLen] = '\0';

		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; ++j)
			FileIO.sStageList[i].actNumber[j] = (char)FileIO.ReadByte();
		FileIO.sStageList[i].actNumber[strLen] = '\0';

		byte count = FileIO.ReadByte();
		for (int j = 0; j < count; ++j)
			FileIO.ReadByte();
		dummy = FileIO.ReadByte();
	}

	// Read bonus stages
	FileIO.noBonusStages = fgetc(f);
	for (int i = 0; i < FileIO.noBonusStages; ++i)
	{
		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; j++)
			FileIO.bStageList[i].stageFolderName[j] = (char)FileIO.ReadByte();
		FileIO.bStageList[i].stageFolderName[strLen] = '\0';

		strLen = FileIO.ReadByte();
		for (int j = 0; j < strLen; ++j)
			FileIO.bStageList[i].actNumber[j] = (char)FileIO.ReadByte();
		FileIO.bStageList[i].actNumber[strLen] = '\0';

		byte count = FileIO.ReadByte();
		for (int j = 0; j < count; ++j)
			FileIO.ReadByte();
		dummy = FileIO.ReadByte();
	}*/
	fclose(f);
	return commonObjectsCount;
}