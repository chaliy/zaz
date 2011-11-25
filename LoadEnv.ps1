gci .\packages -Recurse -Filter "tools" | %{	
	if (!($env:path -like "*$_*")){
		write-host Add $_
		$env:path = ($env:path + ";" + $_)
	}
}

# Register URL for tests
$res = netsh http add urlacl url=http://+:9303/ user=everyone
if (!($res -like "*Cannot create a file when that file already exists.*") -and !($res -like "*URL reservation successfully added*")){
	write-warning @"
HttpSys is not correctly configured to run tests

$res

Run within elevated command prompt:
    netsh http add urlacl url=http://+:9303/ user=everyone
"@
}