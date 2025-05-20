echo Do app-post-build.bat
rem appsettings.json rewrite-job
if exist "%projdir%\appsettings.%projcfg%.json"  (
	echo exist [appsettings.%projcfg%.json]
	rem echo "file : appsettings.%projcfg%.json is exist"  >"%projout%\appsettings-%projcfg%.txt"
	rem set >>"%projout%\appsettings-%projcfg%.txt"
	copy "%projdir%\appsettings.%projcfg%.json" "%projout%\appsettings.json"
) else (
if exist "%projdir%\appsettings.json" (
	echo exist [appsettings.json]
	rem del /Q "%projout%\appsettings-%projcfg%.txt"
	copy "%projdir%\appsettings.json" "%projout%\appsettings.json"
	)
)

exit 0
set projdir="$(ProjectDir)"
set projcfg=$(Configuration)
set projout="$(OutDir)"
rem call app-post-build.bat

