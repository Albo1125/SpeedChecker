fx_version 'cerulean'
games { 'gta5' }

author 'Albo1125'
url 'https://github.com/Albo1125/SpeedChecker'

--file 'weaponmarksmanpistol.meta'
--data_file 'WEAPONINFO_FILE_PATCH' 'weaponmarksmanpistol.meta'

client_script "SpeedChecker.net.dll"
server_script "vars.lua"
server_script "sv_SpeedChecker.lua"