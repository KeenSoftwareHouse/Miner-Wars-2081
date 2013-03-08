@echo off

SET FrameworkPath="%ProgramFiles%\Reference Assemblies\Microsoft\Framework\v3.5"


IF "%CommonProgramFiles(x86)%"=="" (
	SET TextTransformPath="%CommonProgramFiles%\Microsoft Shared\TextTemplating\10.0\TextTransform.exe"
) ELSE (
	SET TextTransformPath="%CommonProgramFiles(x86)%\Microsoft Shared\TextTemplating\10.0\TextTransform.exe"
)

echo Framework: %FrameworkPath%
echo Text Template: %TextTransformPath%

echo %TextTransformPath% -out "%1.cs" -P "%2" -P %FrameworkPath% "%1.tt"
%TextTransformPath% -out "%1.cs" -P "%2" -P %FrameworkPath% "%1.tt"