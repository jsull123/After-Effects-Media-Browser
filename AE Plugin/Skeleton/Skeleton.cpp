#include "Skeleton.h"

static A_long				S_idle_count = 0L;

static AEGP_Command S_my_command = 0L;

AEGP_PluginID S_my_id = 0L;

SPBasicSuite* sP = NULL;

HANDLE hReadPipe, hWritePipe;

PROCESS_INFORMATION pi;

std::string importScript = 
u8"function main(){\n"
u8"    app.beginUndoGroup(\"import item\");\n"
u8"\n"
u8"    var filepath = input.split(\"!\")[2];\n"
u8"    var activeFolderName = input.split(\"!\")[1];\n"
u8"    var pathparts = filepath.split('/');\n"
u8"    var filename = pathparts[pathparts.length-1];\n"
u8"\n"
u8"    var fileItem = null;\n"
u8"\n"
u8"    for(var i = 1; i <= app.project.numItems; i++){\n"
u8"        if (app.project.item(i).name == filename){\n"
u8"            fileItem = app.project.item(i);\n"
u8"            break;\n"
u8"        }\n"
u8"    }\n"
u8"\n"
u8"    if (fileItem == null){  \n"
u8"        var folders = new Array();\n"
u8"        var pastActive = false;  \n"
u8"        for (var f in pathparts){\n"
u8"            if (pathparts[f] == filename){\n"
u8"                break;\n"
u8"            }\n"
u8"            if (pastActive){\n"
u8"                folders.push(pathparts[f]);\n"
u8"            }\n"
u8"            if (pathparts[f] == activeFolderName){\n"
u8"                folders.push(pathparts[f]);\n"
u8"                pastActive = true;\n"
u8"            }\n"
u8"        }\n"
u8"        \n"
u8"        var folderItem = null;\n"
u8"        var parentFolderItem = null;\n"
u8"        for (var f = 0; f < folders.length; f++){\n"
u8"            if (parentFolderItem == null){\n"
u8"                folderItem = findOrCreate(folders[f]);\n"
u8"                parentFolderItem = folderItem;\n"
u8"            }else{\n"
u8"                folderItem = findOrCreateWithParent(folders[f], parentFolderItem);\n"
u8"                parentFolderItem = folderItem;\n"
u8"            }\n"
u8"        }\n"
u8"        \n"
u8"        fileItem = app.project.importFile(new ImportOptions((File(filepath))));\n"
u8"        fileItem.parentFolder = folderItem;\n"
u8"    }\n"
u8"\n"
u8"    var comp = getComp();\n"
u8"    \n"
u8"    if (comp != null){\n"
u8"        comp.layers.add(fileItem);\n"
u8"    }\n"
u8"\n"
u8"    app.endUndoGroup();\n"
u8"}\n"
u8"\n"
u8"function getComp(){\n"
u8"    var activeItem = app.project.activeItem;\n"
u8"    if (activeItem == null){\n"
u8"        return null;\n"
u8"    }\n"
u8"    if (/Composition|Komposition|Composición|Composizione|コンポジション|컴포지션|Composição|Композиция|合成/.test(activeItem.typeName)) {\n"
u8"        return activeItem;\n"
u8"    }\n"
u8"    return null;\n"
u8"}\n"
u8"\n"
u8"function findOrCreate(folderName){\n"
u8"    for(var i = 1; i <= app.project.numItems; i++){\n"
u8"        if (/Folder|Ordner|Carpeta|Dossier|Cartella|フォルダー|폴더|Pasta|Папка|文件夹/.test(app.project.item(i).typeName)){\n"
u8"            if (app.project.item(i).name == folderName){\n"
u8"                return app.project.item(i);\n"
u8"            }\n"
u8"        }\n"
u8"    }\n"
u8"    return app.project.items.addFolder(folderName);\n"
u8"}\n"
u8"\n"
u8"function findOrCreateWithParent(folderName, parentFolderItem){\n"
u8"    for(var i = 1; i <= app.project.numItems; i++){\n"
u8"        if (/Folder|Ordner|Carpeta|Dossier|Cartella|フォルダー|폴더|Pasta|Папка|文件夹/.test(app.project.item(i).typeName)){\n"
u8"            if (app.project.item(i).name == folderName && app.project.item(i).parentFolder == parentFolderItem){\n"
u8"                return app.project.item(i);\n"
u8"            }\n"
u8"        }\n"
u8"    }\n"
u8"    var newFolder = app.project.items.addFolder(folderName);\n"
u8"    newFolder.parentFolder = parentFolderItem;\n"
u8"    return newFolder;\n"
u8"}\n"
u8"\n"
u8"main();\n";

// Death
static A_Err
DeathHook(
	AEGP_GlobalRefcon	plugin_refconP,
	AEGP_DeathRefcon	refconP)
{
	A_Err	err = A_Err_NONE;
	AEGP_SuiteHandler	suites(sP);

	A_char report[AEGP_MAX_ABOUT_STRING_SIZE] = { '\0' };

	suites.ANSICallbacksSuite1()->sprintf(report, STR(StrID_IdleCount), S_idle_count);

	return err;
}

// Idle
static A_Err
IdleHook(
	AEGP_GlobalRefcon	plugin_refconP,
	AEGP_IdleRefcon		refconP,
	A_long* max_sleepPL)
{
	A_Err	err = A_Err_NONE;
	AEGP_SuiteHandler suites(sP);
	S_idle_count++;

	if (pi.hProcess) {
		if (WaitForSingleObject(pi.hProcess, 0) == WAIT_OBJECT_0) {
			CloseHandle(pi.hProcess);
			CloseHandle(pi.hThread);
			if (hReadPipe) {
				CloseHandle(hReadPipe);
			}
			ZeroMemory(&pi, sizeof(pi));
		}
		else {
			char buffer[256];
			DWORD bytesRead, bytesAvailable, bytesLeft;

			if (PeekNamedPipe(hReadPipe, NULL, 0, NULL, &bytesAvailable, &bytesLeft) && bytesAvailable > 0) {
				// If data is available, read it
				if (ReadFile(hReadPipe, buffer, sizeof(buffer) - 1, &bytesRead, NULL) && bytesRead != 0) {
					if (strncmp(buffer, "Import", 6) == 0) {
						buffer[bytesRead - 1] = '\0';
						buffer[bytesRead - 2] = '\0';
						for (int i = 0; i < 256; i++) {
							if (buffer[i] == '\\') {
								buffer[i] = '/';
							}
						}
						std::string script = "var input = \"" + std::string(buffer) + "\"\n" + importScript;
						suites.UtilitySuite6()->AEGP_ExecuteScript(S_my_id, script.c_str(), true, 0, 0);
					}
				}
			}
		}
	}

	return err;
}

static A_Err
UpdateMenuHook(
	AEGP_GlobalRefcon		plugin_refconPV,	/* >> */
	AEGP_UpdateMenuRefcon	refconPV,			/* >> */
	AEGP_WindowType			active_window)		/* >> */
{
	A_Err 				err = A_Err_NONE;

	AEGP_SuiteHandler	suites(sP);

	ERR(suites.CommandSuite1()->AEGP_EnableCommand(S_my_command));

	return err;
}

static A_Err
CommandHook(
	AEGP_GlobalRefcon	plugin_refconPV,
	AEGP_CommandRefcon	refconPV,
	AEGP_Command		command,
	AEGP_HookPriority	hook_priority,
	A_Boolean			already_handledB,
	A_Boolean* handledPB)
{
	A_Err			err = A_Err_NONE;

	AEGP_SuiteHandler	suites(sP);

	if (S_my_command == command) {
		*handledPB = TRUE;

		SECURITY_ATTRIBUTES sa = { sizeof(SECURITY_ATTRIBUTES), NULL, TRUE };

		// Create an anonymous pipe
		if (!CreatePipe(&hReadPipe, &hWritePipe, &sa, 0)) {;
			return err;
		}

		STARTUPINFO si;
		ZeroMemory(&si, sizeof(si));
		si.cb = sizeof(si);
		si.hStdOutput = hWritePipe;
		si.hStdError = hWritePipe;
		si.dwFlags |= STARTF_USESTDHANDLES;

		ZeroMemory(&pi, sizeof(pi));

		//LPCSTR applicationName = "..\\FileBrowser\\MediaManager2.exe";

		// Path starts at C:\Program Files\Adobe\Adobe After Effects 2024\Support Files
		LPCSTR applicationName = "..\\..\\Common\\Plug-ins\\7.0\\MediaCore\\smm\\FileBrowser\\MediaManager2.exe";
	
		// Create the process
		BOOL success = CreateProcess(
			NULL,                  // Application name
			(LPSTR)applicationName, // Command line
			NULL,                  // Process security attributes
			NULL,                  // Primary thread security attributes
			TRUE,                  // Handles are inherited
			0,                     // Creation flags
			NULL,                  // Use parent's environment
			NULL,                  // Use parent's starting directory
			&si,                   // Pointer to STARTUPINFO
			&pi                    // Pointer to PROCESS_INFORMATION
		);
		if (!success) {
			CloseHandle(hReadPipe);
			CloseHandle(hWritePipe);
			return err;
		}

		// not used currently
		CloseHandle(hWritePipe);
	}

	return err;
}

A_Err
EntryPointFunc(
	struct SPBasicSuite* pica_basicP,
	A_long				 	major_versionL,
	A_long					minor_versionL,
	AEGP_PluginID			aegp_plugin_id,
	AEGP_GlobalRefcon* global_refconV)
{
	S_my_id = aegp_plugin_id;
	A_Err 					err = A_Err_NONE,
		err2 = A_Err_NONE;

	sP = pica_basicP;

	AEGP_SuiteHandler suites(pica_basicP);

	ERR(suites.CommandSuite1()->AEGP_GetUniqueCommand(&S_my_command));

	ERR(suites.CommandSuite1()->AEGP_InsertMenuCommand(S_my_command, "smm", AEGP_Menu_FILE, AEGP_MENU_INSERT_AT_TOP));

	ERR(suites.RegisterSuite5()->AEGP_RegisterCommandHook(S_my_id,
		AEGP_HP_BeforeAE,
		AEGP_Command_ALL,
		CommandHook,
		0));

	ERR(suites.RegisterSuite5()->AEGP_RegisterUpdateMenuHook(S_my_id, UpdateMenuHook, 0));

	ERR(suites.RegisterSuite5()->AEGP_RegisterDeathHook(S_my_id, DeathHook, NULL));

	ERR(suites.RegisterSuite5()->AEGP_RegisterIdleHook(S_my_id, IdleHook, NULL));

	if (err) { // not !err, err!
		ERR2(suites.UtilitySuite3()->AEGP_ReportInfo(S_my_id, "fail"));
	}
	return err;
}