Function Get-TestMessage{
	param([string] $context)
	return "Welcome from context {0}" -f $context
}

Function Set-TestMessage{
	param([string] $context)
	$message = "Welcome from context {0}" -f $context
	Write-Host "Setting the welcome message as {0}" -f $message
	return $message
}