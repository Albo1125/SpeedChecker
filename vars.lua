local useWhitelist = false
local authorizedIdentifiers = {
	-- Add steam/license/discord/other identifiers here
    "license:123",
	
}

function isAuthorized(player)
	if not useWhitelist then return true end
    for i,id in ipairs(authorizedIdentifiers) do
        for x,pid in ipairs(GetPlayerIdentifiers(player)) do
            if pid == id then
                return true
            end
        end
    end
    return false
end