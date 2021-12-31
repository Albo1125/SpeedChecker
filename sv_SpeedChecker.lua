RegisterCommand('speedgun', function(source, args, rawCommand)
	TriggerClientEvent("SpeedChecker:ToggleSpeedgun", source)
end, false)

RegisterServerEvent("SpeedChecker:GetSpeed")
AddEventHandler('SpeedChecker:GetSpeed', function(requester, target)
	TriggerClientEvent("SpeedChecker:GetSpeedCl", target, requester)
end)

RegisterServerEvent("SpeedChecker:PushSpeed")
AddEventHandler('SpeedChecker:PushSpeed', function(requester, speed)
	TriggerClientEvent("SpeedChecker:PushSpeedCl", requester, speed)
end)

RegisterServerEvent("SpeedChecker:svProvidaAuth")
AddEventHandler('SpeedChecker:svProvidaAuth', function(requester)
	if isAuthorized(requester) then
		TriggerClientEvent("SpeedChecker:ProvidaAuth", requester, true)
	else
		TriggerClientEvent("SpeedChecker:ProvidaAuth", requester, false)
	end
end)