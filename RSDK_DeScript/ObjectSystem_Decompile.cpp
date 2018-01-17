#include "ObjectSystem.h"
#include "opcodes.h"
#include "variables.h"

const byte ObjectSystem::scriptOpcodeSizes[]
{
	0, 2, 2, 2, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2,
	2, 2, 2, 3, 3, 3, 3, 3, 3, 0, 0, 3, 3, 3, 3, 3,
	3, 0, 2, 0, 0, 2, 2, 2, 2, 2, 5, 5, 3, 4, 7, 1,
	1, 1, 3, 3, 4, 7, 7, 3, 6, 6, 5, 3, 4, 3, 7, 2,
	1, 4, 4, 1, 4, 3, 4, 0, 8, 5, 5, 4, 2, 0, 0, 0,
	0, 0, 3, 1, 0, 2, 1, 3, 4, 4, 1, 0, 2, 1, 1, 0,
	1, 2, 4, 4, 2, 2, 2, 4, 3, 1, 0, 6, 4, 4, 4, 3,
	3, 0, 0, 1, 2, 3, 3, 4, 2, 4, 2, 0, 0, 1, 3, 7,
	5, 2, 2, 2, 1, 1, 4
};

void _SetArrayValue(char *strOut, const char *strIn, int index) {
	int point = -1;
	for (int i = 0; strIn[i]; i++) {
		if (strIn[i] == '.') {
			point = i;
			break;
		}
	}
	if (point >= 0) {
		memcpy(strOut, strIn, point);
		sprintf(strOut + point, "[%i]%s", index, strIn + point);
	}
	else
		sprintf(strOut, "%s[%i]", strIn, index);
}

void ObjectSystem::Decompile() {
	for (int i = 1; sourceNames[i][0] != '\0'; i++) {
		FILE *f = fopen(sourceNames[i], "w");
		if (f != NULL) {
			if (i == 11) continue;

			ObjectScript& objectScript = objectScriptList[i];
			printf("[%3i] Decompiling %s: ", i, typeNames[i]);
			if (i < m_stageVarsIndex) {
				for (int j = 0; j < m_stageVarsIndex; j++)
					fprintf(f, "#define %s %i\n", typeNames[j], j);
			}
			else {
				for (int j = 0; typeNames[j][0]; j++)
					fprintf(f, "#define %s %i\n", typeNames[j], j);
			}
			fprintf(f, "\n\n");
			printf("Main script, ");
			DecompileScript(f, objectScript.mainScript, objectScript.mainJumpTable, 0);
			printf("Player script, ");
			DecompileScript(f, objectScript.playerScript, objectScript.playerJumpTable, 1);
			printf("Draw script, ");
			DecompileScript(f, objectScript.drawScript, objectScript.drawJumpTable, 2);
			printf("Startup script.\n");
			DecompileScript(f, objectScript.startupScript, objectScript.startupJumpTable, 3);
			fclose(f);
		}
		else {
			printf("Error on creating file %s\n", sourceNames[i]);
		}
	}
}


void ObjectSystem::DecompileStatement(FILE* f, StateScriptEngine& state) {
	int objectLoop = 0;
	int index1 = 0;
	bool flag = false;
	fputc('\n', f);
	while (!flag) {
		int num2 = 0;
		int opcode = scriptData[state.scriptCodePtr++];
		int paramsCount = scriptOpcodeSizes[opcode];

		char variableName[10][0x100];
		int variableIndex;
		for (int i = 0; i < paramsCount; i++) {
			int paramId = scriptData[state.scriptCodePtr++];
			switch (paramId) {
			case 1: // Read value from RSDK
				switch (scriptData[state.scriptCodePtr++]) {
				case 0:
					index1 = objectLoop;
					strcpy(variableName[i], VARIABLE_NAME[scriptData[state.scriptCodePtr++]]);
					break;
				case 1: // ARRAY
					if (scriptData[state.scriptCodePtr++] == 1)
						index1 = scriptEng.arrayPosition[scriptData[state.scriptCodePtr++]];
					else
						index1 = scriptData[state.scriptCodePtr++];
					num2 += 2;
					_SetArrayValue(variableName[i], VARIABLE_NAME[scriptData[state.scriptCodePtr++]], index1);
					break;
				case 2:
					if (scriptData[state.scriptCodePtr++] == 1)
						index1 = objectLoop + scriptEng.arrayPosition[scriptData[state.scriptCodePtr++]];
					else
						index1 = objectLoop + scriptData[state.scriptCodePtr++];
					num2 += 2;
					strcpy(variableName[i], VARIABLE_NAME[scriptData[state.scriptCodePtr++]]);
					break;
				case 3:
					if (scriptData[state.scriptCodePtr++] == 1)
						index1 = objectLoop - scriptEng.arrayPosition[scriptData[state.scriptCodePtr++]];
					else
						index1 = objectLoop - scriptData[state.scriptCodePtr++];
					num2 += 2;
					strcpy(variableName[i], VARIABLE_NAME[scriptData[state.scriptCodePtr++]]);
					break;
				}
				num2 += 3;
				break;
			case 2: // Read constant value from bytecode
				scriptEng.operands[i] = scriptData[state.scriptCodePtr++];
				sprintf(variableName[i], "%i", scriptEng.operands[i]);
				num2 += 2;
				break;
			case 3: // Read string
				char tmp[0x100];
				num2++;
				int strLen = scriptData[state.scriptCodePtr];
				for (int j = 0; j < strLen; j += 4)
				{
					state.scriptCodePtr++;
					num2++;
					tmp[j + 0] = (scriptData[state.scriptCodePtr] >> 24);
					tmp[j + 1] = (scriptData[state.scriptCodePtr] & 0x00FFFFFF) >> 16;
					tmp[j + 2] = (scriptData[state.scriptCodePtr] & 0x0000FFFF) >> 8;
					tmp[j + 3] = (scriptData[state.scriptCodePtr] & 0x000000FF);
				}
				tmp[strLen] = '\0';
				sprintf(variableName[i], "\"%s\"", tmp);
				if ((strLen & 3) == 0)
				{
					state.scriptCodePtr += 2;
					num2 += 2;
					break;
				}
				state.scriptCodePtr++;
				num2++;
				break;
			}
		}

		// Check what opcodes terminates a statement
		switch (opcode) {
		case 0x00: // endsub
		case 0x1A: // endif
		case 0x21: // loop
			state.deep--;
			for (int i = 0; i < state.deep; i++) fprintf(f, "\t");
			flag = true;
			break;
		case 0x23: // break
			flag = true;
			for (int i = 0; i < state.deep; i++) fprintf(f, "\t");
			state.deep--;
			// do a peek if the next statement is an endswitch
			if (scriptData[state.scriptCodePtr] == 0x24) {
				state.isSwitchEnd = true;
			}
			break;
		case 0x19: // else
			for (int i = 0; i < state.deep - 1; i++) fprintf(f, "\t");
			break;
		default:
			for (int i = 0; i < state.deep; i++) fprintf(f, "\t");
			break;
		}

		// Use specific operands in some situation
		switch (opcode) {
		case 0x55: // PlaySfx
		case 0x56: // StopSfx
			//sprintf_s(variableName[0], "\"%s\"", sfxNames[scriptEng.operands[0]]);
			break;
		}


		if (opcode >= 134) {
			fprintf(f, "ERROR %08X:%08X", state.scriptCodePtr, opcode);
			state.error = true;
			return;
		}
		const char* operand = opcodeList[opcode];
		switch (opcode) {
		case 0x00: fprintf(f, "end sub"); flag = true; break;
		case 0x01: fprintf(f, "%s = %s", variableName[0], variableName[1]); break;
		case 0x02: fprintf(f, "%s += %s", variableName[0], variableName[1]); break;
		case 0x03: fprintf(f, "%s -= %s", variableName[0], variableName[1]); break;
		case 0x04: fprintf(f, "%s++", variableName[0]); break;
		case 0x05: fprintf(f, "%s--", variableName[0]); break;
		case 0x06: fprintf(f, "%s *= %s", variableName[0], variableName[1]); break;
		case 0x07: fprintf(f, "%s /= %s", variableName[0], variableName[1]); break;
		case 0x08: fprintf(f, "%s >>= %s", variableName[0], variableName[1]); break;
		case 0x09: fprintf(f, "%s <<= %s", variableName[0], variableName[1]); break;
		case 0x0A: fprintf(f, "%s &= %s", variableName[0], variableName[1]); break;
		case 0x0B: fprintf(f, "%s |= %s", variableName[0], variableName[1]); break;
		case 0x0C: fprintf(f, "%s ^= %s", variableName[0], variableName[1]); break;
		case 0x0D: fprintf(f, "%s %%= %s", variableName[0], variableName[1]); break;
		case 0x0E: fprintf(f, "%s =- %s", variableName[0], variableName[0]); break;
		case 0x0F: fprintf(f, "%s != %s", variableName[0], variableName[1]); break;
		case 0x10: fprintf(f, "%s <= %s", variableName[0], variableName[1]); break;
		case 0x11: fprintf(f, "%s >= %s", variableName[0], variableName[1]); break;
		case 0x12: fprintf(f, "%s == %s", variableName[0], variableName[1]); break;
		case 0x13: fprintf(f, "if %s == %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x14: fprintf(f, "if %s > %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x15: fprintf(f, "if %s >= %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x16: fprintf(f, "if %s < %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x17: fprintf(f, "if %s <= %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x18: fprintf(f, "if %s != %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x19: fprintf(f, "else"); break;
		case 0x1A: fprintf(f, "endif"); break;
		case 0x1B: fprintf(f, "while %s == %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x1C: fprintf(f, "while %s > %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x1D: fprintf(f, "while %s >= %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x1E: fprintf(f, "while %s < %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x1F: fprintf(f, "while %s <= %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x20: fprintf(f, "while %s != %s", variableName[1], variableName[2]);
			DecompileStatement(f, state.IncDeep());
			break;
		case 0x21: fprintf(f, "loop"); break;
		case 0x22:
			fprintf(f, "switch %s\n", variableName[1]);
			for (int i = 0; !state.isSwitchEnd; i++) {
				for (int j = 0; j < state.deep; j++) fprintf(f, "\t");
				fprintf(f, "case %i:", i);
				DecompileStatement(f, state.IncDeep());
			}
			state.isSwitchEnd = false;
			break;
		case 0x23: fprintf(f, "break"); break;
		case 0x24: fprintf(f, "endswitch"); break;
		default:
			fprintf(f, "%s(", operand);
			switch (paramsCount) {
			case 1:
				fprintf(f, "%s", variableName[0]);
				break;
			case 2:
				fprintf(f, "%s, %s", variableName[0], variableName[1]);
				break;
			case 3:
				fprintf(f, "%s, %s, %s", variableName[0], variableName[1], variableName[2]);
				break;
			case 4:
				fprintf(f, "%s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3]);
				break;
			case 5:
				fprintf(f, "%s, %s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3], variableName[4]);
				break;
			case 6:
				fprintf(f, "%s, %s, %s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3], variableName[4], variableName[5]);
				break;
			case 7:
				fprintf(f, "%s, %s, %s, %s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3], variableName[4], variableName[5], variableName[6]);
				break;
			case 8:
				fprintf(f, "%s, %s, %s, %s, %s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3], variableName[4], variableName[5], variableName[6], variableName[7]);
				break;
			case 9:
				fprintf(f, "%s, %s, %s, %s, %s, %s, %s, %s, %s", variableName[0], variableName[1], variableName[2], variableName[3], variableName[4], variableName[5], variableName[6], variableName[7], variableName[8]);
				break;
			}
			fprintf(f, ")");
			break;
		}
		if (!state.isSwitchEnd)
			fputc('\n', f);
	}
}
void ObjectSystem::DecompileScript(FILE* f, int scriptCodePtr, int jumpTablePtr, int scriptSub) {
	const char* strFuncName;
	switch (scriptSub) {
	case 0: strFuncName = "ObjectMain";
		break;
	case 1: strFuncName = "ObjectPlayer";
		break;
	case 2: strFuncName = "ObjectDraw";
		break;
	case 3: strFuncName = "ObjectStartup";
		break;
	default: strFuncName = "Function";
		break;
	}
	fprintf(f, "sub %s\n", strFuncName);

	StateScriptEngine state;
	state.scriptCodePtr = scriptCodePtr;
	state.jumpTablePtr = jumpTablePtr;
	state.scriptSub = scriptSub;
	state.deep = 1;
	state.isSwitchEnd = false;
	state.error = false;

	DecompileStatement(f, state);
	fprintf(f, "\n");
}