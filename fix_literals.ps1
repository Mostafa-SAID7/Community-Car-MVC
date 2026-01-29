$f = "c:\Users\memos\OneDrive\Desktop\Projects\Community Car MVC\src\CommunityCar.Application\Services\Account\UserAnalyticsService.cs"
$c = Get-Content $f -Raw -Encoding UTF8
$c = $c.Replace('`r`n', "`r`n")
Set-Content $f $c -Encoding UTF8
