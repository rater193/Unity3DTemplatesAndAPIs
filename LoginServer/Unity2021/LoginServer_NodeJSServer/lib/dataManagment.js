
let dataManagment = {};

dataManagment.randomData = {}
dataManagment.randomData.characters = [
    "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",
    "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
    "0","1","2","3","4","5","6","7","8","9",
    "_",".","(",")","[","]"
]

dataManagment.randomData.getRandomChar = function() {
    return dataManagment.randomData.characters[Math.round(Math.random() * (dataManagment.randomData.characters.length-1))];
}

dataManagment.randomData.getRandomString = function(strLen) {
    let ret = "";
    while(strLen > 0) {
        ret = ret+dataManagment.randomData.getRandomChar();
        strLen -= 1;
    }
    return ret;
}

dataManagment.randomData.getCharCode = function(saltKey, place) {
    return saltKey.charCodeAt(place % saltKey.length);
}

//Salt managment
dataManagment.salt = {};
dataManagment.salt.getNewKey = function() {
    return dataManagment.randomData.getRandomString(8);
};

//Salts the text with the salt key
dataManagment.salt.salt = function(message, saltKey) {
    return saltKey + message;
};


//Encryption managment
dataManagment.encrypt = function(message, saltKey, encryptionKey) {
    let saltedData = dataManagment.salt.salt(message, saltKey);
    let key = dataManagment.salt.salt(encryptionKey, saltKey);

    let ret = "";
    for(let place = 0; place < message.length; place++) {
        //dataManagment.randomData.getCharCode(saltKey, place);
        let charOffset = dataManagment.randomData.getCharCode(key, place);
        ret += String.fromCharCode(message.charCodeAt(place) + charOffset);
    }
    return ret;
}

dataManagment.decrypt = function(message, saltKey, encryptionKey) {
    let saltedData = dataManagment.salt.salt(message, saltKey);
    let key = dataManagment.salt.salt(encryptionKey, saltKey);

    let ret = "";
    for(let place = 0; place < message.length; place++) {
        //dataManagment.randomData.getCharCode(saltKey, place);
        let charOffset = dataManagment.randomData.getCharCode(key, place);
        ret += String.fromCharCode(message.charCodeAt(place) - charOffset);
    }
    return ret;
}

module.exports = dataManagment;

