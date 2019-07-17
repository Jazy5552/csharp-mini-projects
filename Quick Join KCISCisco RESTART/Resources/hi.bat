@echo off 
cls 
hostname 
echo You may disconnect USB now. 
timeout 5 
net user KCISCisco /add 
net localgroup administrators KCISCisco /add 
reg add "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Session Manager\Power" /v HiberbootEnabled /t REG_DWORD /d 0 /f
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v dontdisplaylastusername /t REG_DWORD /d 1 /f
powershell -command "$user = [adsi]\"WinNT://./KCISCisco\";$user.UserFlags.value = $user.UserFlags.value -bor 0x10000;$user.CommitChanges()" 
powershell -command "$user=\"mykendall\cisacct\";$pass=ConvertTo-SecureString \"Officedepot\" -AsPlainText -Force;$DomainCred=New-Object System.Management.Automation.PSCredential($user,$pass);Add-Computer -DomainName \"mykendall\" -credential $DomainCred -PassThru" 
rem http://technet.microsoft.com/en-us/library/ff793406.aspx 
rem http://technet.microsoft.com/en-us/library/ee624348.aspx 
md c:\temp 
cscript c:\windows\system32\slmgr.vbs /skms kmshost001.mdc.edu 
cscript c:\windows\system32\slmgr.vbs /ipk 33PXH-7Y6KF-2VJC9-XBBR8-HVTHH 
cscript c:\windows\system32\slmgr.vbs /ato 
cscript "c:\program files\microsoft office\office14\ospp.vbs" /sethst:kmshost001.mdc.edu 
cscript "c:\program files\microsoft office\office14\ospp.vbs" /unpkey:B3WJ3 
start winword 
timeout 20 
taskkill /im winword.exe 
timeout 10 
cmd /k "del c:\windows\temp\hi.bat & cls & shutdown /f /r /t 0" 
