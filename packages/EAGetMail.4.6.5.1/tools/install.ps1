param($installPath, $toolsPath, $package, $project)

$files = Get-ChildItem $installPath -Include EAGetMail*.dll, EAGetMail*.winmd -recurse
foreach($file in $files)
{
    $file.CreationTime = Get-Date
    $file.LastWriteTime = Get-Date
    $file.LastAccessTime = Get-Date
}