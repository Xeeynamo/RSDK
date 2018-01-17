#include "ObjectSystem.h"

AnimationFileList AnimationSystem::GetDefaultAnimationRef() {
	AnimationFileList animationFile;
	animationFile.fileName[0] = '\0';
	animationFile.numAnimations = 0;
	animationFile.aniListOffset = 0;
	animationFile.cbListOffset = 0;
	return animationFile;
}