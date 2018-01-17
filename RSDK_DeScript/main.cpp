#include <stdio.h>
#include <stdlib.h>
#include "ObjectSystem.h"

int main() {
	static const char *STAGES[] = {
		"Title", "Menu", "TAttack", "Secrets", "DAgdn", "Help", "LBoards", "Trophy", "Upsell", "Credits",
		"R11A", "R11B", "R11C", "R11D","R12A", "R12B", "R12C", "R12D", "R13C", "R13D",
		//"R31A", "R31B", "R31C", "R31D","R32A", "R32B", "R32C", "R32D", "R33C", "R33D",
		//"R41A", "R41B", "R41C", "R41D","R42A", "R42B", "R42C", "R42D", "R43C", "R43D",
		//"R51A", "R51B", "R51C", "R51D","R52A", "R52B", "R52C", "R52D", "R53C", "R53D",
		//"R61A", "R61B", "R61C", "R61D","R62A", "R62B", "R62C", "R62D", "R63C", "R63D",
		//"R71A", "R71B", "R71C", "R71D","R72A", "R72B", "R72C", "R72D", "R73C", "R73D",
		"R81A", "R81B", "R81C", "R81D","R82A", "R82B", "R82C", "R82D", "R83C", "R83D",
		"SS1", "SS2", "SS3", "SS4", "SS5", "SS6", "SS7", "SS8",
		"",
	};


	ObjectSystem *objsys = new ObjectSystem();
	for (int i = 0; STAGES[i][0]; i++) {
		char buf1[0x100], buf2[0x100];
		sprintf_s(buf1, "Data/Stages/%s/StageConfig.bin", STAGES[i]);
		sprintf_s(buf2, "Data/Scripts/Bytecode/%s.bin", STAGES[i]);

		printf("Decompiling stage %s\n", STAGES[i]);
		objsys->ClearScriptData();
		objsys->LoadByteCodeFile("Data/Scripts/ByteCode/GlobalCode.bin", 1);
		objsys->LoadStageFile(buf1, buf2);
		objsys->Decompile();
	}
	delete objsys;

	printf("\nPress any key to exit.");
	getchar();
}