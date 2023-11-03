﻿
mgfxc  .\disable.fx .\disable_gl.mgfx /Profile:OpenGL

mgfxc  .\outline.fx .\outline_gl.mgfx /Profile:OpenGL



SET TWOMGFX="C:\Users\liuya\.nuget\packages\dotnet-mgcb-editor-windows\3.8.1.303\tools\net6.0\any\mgcb-editor-windows-data\mgcb-editor-windows.exe"

set expanded_list=
for /f "tokens=*" %%F in ('dir /b *.fx') do call set expanded_list=%%expanded_list%% "%%F"

call %TWOMGFX% %expanded_list% /Platform:WindowsStoreApp /outputDir:DirectX
call %TWOMGFX% %expanded_list% /Platform:Android /outputDir:OpenGL
