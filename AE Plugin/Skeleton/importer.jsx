var input = "Import!Assets!C:/Users/DeMarcus Cousins/Documents/After Effects Projects/Assets/Images/Film/film.png"
function main(){
    app.beginUndoGroup("import item")

    var filepath = input.split("!")[2];
    var activeFolderName = input.split("!")[1];
    var pathparts = filepath.split('/');
    var filename = pathparts[pathparts.length-1];

    var fileItem = null;

    for(var i = 1; i <= app.project.numItems; i++){
        if (app.project.item(i).name == filename){
            fileItem = app.project.item(i);
            break;
        }
    }

    if (fileItem == null){  
        var folders = new Array();
        var pastActive = false;  
        for (var f in pathparts){
            if (pathparts[f] == filename){
                break;
            }
            if (pastActive){
                folders.push(pathparts[f]);
            }
            if (pathparts[f] == activeFolderName){
                folders.push(pathparts[f]);     
                pastActive = true;
            }
        }
        
        var folderItem = null;
        var parentFolderItem = null;
        for (var f = 0; f < folders.length; f++){
            if (parentFolderItem == null){
                folderItem = findOrCreate(folders[f]);
                parentFolderItem = folderItem;
            }else{
                folderItem = findOrCreateWithParent(folders[f], parentFolderItem);
                parentFolderItem = folderItem;
            }
        }
        
        fileItem = app.project.importFile(new ImportOptions((File(filepath))))
        fileItem.parentFolder = folderItem;
    }

    var comp = getComp();
    
    if (comp != null){
        comp.layers.add(fileItem);
    }

    app.endUndoGroup()
}

function getComp(){
    var activeItem = app.project.activeItem;
    if (activeItem == null){
        return null;
    }
    if (/Composition|Komposition|Composición|Composizione|コンポジション|컴포지션|Composição|Композиция|合成/.test(activeItem.typeName)) {
        return activeItem;
    }
    return null;
}

function findOrCreate(folderName){
    for(var i = 1; i <= app.project.numItems; i++){
        if (/Folder|Ordner|Carpeta|Dossier|Cartella|フォルダー|폴더|Pasta|Папка|文件夹/.test(app.project.item(i).typeName)){
            if (app.project.item(i).name == folderName){
                return app.project.item(i);
            }
        }
    }
    return app.project.items.addFolder(folderName);
}

function findOrCreateWithParent(folderName, parentFolderItem){
    for(var i = 1; i <= app.project.numItems; i++){
        if (/Folder|Ordner|Carpeta|Dossier|Cartella|フォルダー|폴더|Pasta|Папка|文件夹/.test(app.project.item(i).typeName)){
            if (app.project.item(i).name == folderName && app.project.item(i).parentFolder == parentFolderItem){
                return app.project.item(i);
            }
        }
    }
    var newFolder = app.project.items.addFolder(folderName);
    newFolder.parentFolder = parentFolderItem;
    return newFolder;
}

main()

