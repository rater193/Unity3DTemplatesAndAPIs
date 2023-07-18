
let db = {};

let storedAccounts = {};
let fs = require("fs");

//Database variables
db.encryptionPassword = "asdT!VXCewRDFV_-,1!"

//Here we are writing functions for exposing the database API

//Here we are saving the data for the account
db.saveAccountData = function(accountData) {
    console.log("Saving account");
    let constructedData = {
        ["salt"]: dataManagment.encrypt(accountData.salt, accountData.name, db.encryptionPassword),
        ["password"]: dataManagment.encrypt(accountData.password, accountData.name, db.encryptionPassword)
    }
    let ret = (dataManagment.encrypt(JSON.stringify(constructedData), accountData.name, db.encryptionPassword));
    fs.writeFileSync("./accounts/"+accountData.name, ret, 'utf8');
    return ret;
};

db.loadAccountData = function(encryptedData, accountName) {
    console.log("Loading account");
    let encryptedJSON = JSON.parse(dataManagment.decrypt(encryptedData, accountName, db.encryptionPassword));
    let deconstructedData = {
        ["name"]: accountName,
        ["salt"]: dataManagment.decrypt(encryptedJSON.salt, accountName, db.encryptionPassword),
        ["password"]: dataManagment.decrypt(encryptedJSON.password, accountName, db.encryptionPassword)
    };

    return deconstructedData;
};

//Creating an account
db.AccountCreate = function(accountName, accountPassword) {
    if(accountName.length > 0 && accountPassword.length > 0) {
        if(!this.AccountExists(accountName)) {
            //TODO: AccountCreate
            let accountSaltKey = dataManagment.salt.getNewKey();

            let account = {
                ["name"]: accountName,
                ["salt"]: accountSaltKey,
                ["password"]: dataManagment.encrypt(accountPassword, accountSaltKey, db.encryptionPassword)
            }
            db.saveAccountData(account);
            storedAccounts[account.name] = account;
            
            return account;
        }
    }
    return undefined;
}

//Loading the account into memory
db.AccountLoad = function(accountName) {
    if(accountName.length > 0) {
        console.log("Loading data");
        if(storedAccounts[accountName]!=null) {
            console.log("Account found");
            return true;
        }else{
            if(fs.existsSync("./accounts/"+accountName)) {
                console.log("Loading account into memory...");
                storedAccounts[accountName] = db.loadAccountData(fs.readFileSync("./accounts/"+accountName, 'utf8'), accountName);
                return true;
            }
        }
        console.log("No account found!");
    }else{
        console.log("Account name has to be greater than 0 characters!");
    }
    return false;
}

//Checking if an account exists
db.AccountExists = function(accountName) {
    return fs.existsSync("./accounts/"+accountName);
}

db.AccountGet = function(accountName) {
    db.AccountLoad(accountName);
    return storedAccounts[accountName];
}

//Here we are initializing the database

module.exports = db;