
let sessionkeys = {}

let keys = ["0","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f"];
let keyIndex = [];
for(let _index = 0; _index < 32; _index += 1) {
    keyIndex[_index] = keys[Math.round(Math.random()*(keys.length-1))];
}

let keyExpireDates = { };
let storedKeys = { };//The key for this is the player name
let playerReference = { };//The key for this is the session key

//Getting a player's session key
sessionkeys.getPlayerKey = function(playername) {
    if(storedKeys[playername]) {
        return storedKeys[playername];
    }
    let newkey = getNextKey();
    keyExpireDates[playername] = { ["time"]: getNewKeyTime(), ["key"]: newkey };
    storedKeys[playername] = newkey;
    playerReference[newkey] = playername;
    return storedKeys[playername];
}


//Checking if a session key is valid, if so it will return the name binded to the key
sessionkeys.checkKey = function(key) {
    console.log("Checking " + key);
    if (playerReference[key]) {
        console.log("Key found");
        keyExpireDates[playerReference[key]].time = getNewKeyTime();
        return playerReference[key];
    }
    return "<UNDEFINED>";
}

let getNewKeyTime = function() {
    return (new Date().getTime() / 1000) + 60 * 60 * 12;
}

//Checks for expired keys
let expiredKeyCheck = function() {
    for (const [key, keyData] of Object.entries(keyExpireDates)) {
        if(new Date().getTime() / 1000 >= keyData.time) {
            console.log("Cleaning up expired key: " + keyData.key);
            delete storedKeys[key];
            delete playerReference[keyData.key];
            delete keyData;
            delete keyExpireDates[key];
        }
    }
}

//Generates the next key in sequence
let getNextKey = function() {
    let ret = "";

    //Here we are generating the key
    for(let index = 0; index < keyIndex.length; index++) {
        ret += keyIndex[index];
    }

    //Here we are increasing the key by one
    for(let index = keyIndex.length-1; index >= 0; index--) {
        let newindex = keys.indexOf(keyIndex[index]);
        //Resetting it, and continuing execution, otherwise, it will just break
        if(newindex >= keys.length-1) {
            keyIndex[index] = keys[0];
        }else{
            keyIndex[index] = keys[newindex+1]
            break;
        }
    }
    return ret;
}

//Here we are exporting out session module
module.exports = sessionkeys;


setInterval(expiredKeyCheck, 60000);