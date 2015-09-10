$appCmd = "C:\Program Files (x86)\IIS Express\appcmd.exe"


$result = Invoke-Command -Command {& $appCmd 'list' 'sites' '/text:SITE.NAME' }



for ($i=0; $i -lt $result.length; $i++)

{
    
	Invoke-Command -Command {& $appCmd 'delete' 'site'  $result[$i] }

}