for /F %%i in (DataFolders.txt) do (
	del /q %%i\data\
	del /q %%i\params.cfg
	copy DatabaseEditor\bin\Debug\data\ %%i\data
	copy DatabaseEditor\bin\Debug\params.cfg %%i\
)
pause
