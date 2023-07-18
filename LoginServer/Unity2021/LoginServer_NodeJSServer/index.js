
dataManagment = require("./lib/dataManagment");
db = require("./lib/dbmanager");
WebSocket = require("ws");
wss = new WebSocket.Server({port: 8080});
sessionkeys = require("./lib/sessionKeyManager");
http = require('http');

//create a server object:
http.createServer(function (req, res) {
    console.log(req.url);
    let url = req.url.slice(1);
    console.log("CHECKING KEY");
    res.write(sessionkeys.checkKey(url)); //write a response to the client
    res.end(); //end the response
}).listen(8081); //the server object listens on port 8080

wss.on('connection', ws => {
    let saltKey = dataManagment.randomData.getRandomString(8);
    let debounce = false;

    ws.on('message', message => {
        if(debounce==false) {
            if(message==saltKey) {
                let replymsg = (dataManagment.encrypt(JSON.stringify({
                    ["action"]: "acceptedKey"
                }), saltKey, saltKey));
                ws.send(replymsg);
                debounce = true;
            }
        }else{
            let data = JSON.parse(dataManagment.decrypt(message, saltKey, saltKey));
            let usr;
            let _p;
            let charat;
            let passwordCheck;
            let salt;

            passwordCheck = function(_p, pass) {
                salt = charat(_p,0)+charat(_p,1)+charat(_p,2)+charat(_p,3)+charat(_p,4)+charat(_p,5)+charat(_p,6)+charat(_p,7);
                
                if(_p==saltKey+dataManagment.encrypt(pass, saltKey, saltKey)) {
                    
                }
                
            }
            
            charat = function(txt, pos) {
                return String.fromCharCode(txt.charCodeAt(pos));
            }
            
            switch(data.action) {
                case "Login":
                    usr = dataManagment.decrypt(data.u, saltKey, saltKey);
                    _p = saltKey+data.p;
                    if(db.AccountExists(usr)) {
                        let acc = db.AccountGet(usr);
                        let __a = dataManagment.decrypt(acc.password, acc.salt, db.encryptionPassword);
                        let _a = charat(__a, 0)+charat(__a, 1)+charat(__a, 2)+charat(__a, 3)+charat(__a, 4)+charat(__a, 5)+charat(__a, 6)+charat(__a, 7);
                        if((_a+dataManagment.encrypt(dataManagment.decrypt(data.p, saltKey, saltKey), _a, _a))==__a) {
                            ws.send(dataManagment.encrypt(JSON.stringify({
                                ["action"]: "SuccessfulLogin",
                                ["sessionKey"]: sessionkeys.getPlayerKey(usr)
                            }), saltKey, saltKey));
                        }else{
                            ws.send(dataManagment.encrypt(JSON.stringify({
                                ["action"]: "InvalidLogin"
                            }), saltKey, saltKey));
                        }
                    }else{
                        ws.send(dataManagment.encrypt(JSON.stringify({
                            ["action"]: "AccountNotRegistered"
                        }), saltKey, saltKey));
                    }
                break;
                case "Register":
                    usr = dataManagment.decrypt(data.u, saltKey, saltKey);
                    _p = saltKey+data.p;

                    if(db.AccountCreate(usr, _p)!=undefined) {
                        ws.send(dataManagment.encrypt(JSON.stringify({
                            ["action"]: "accountRegistered"
                        }), saltKey, saltKey));
                    }else{
                        ws.send(dataManagment.encrypt(JSON.stringify({
                            ["action"]: "accountExists"
                        }), saltKey, saltKey));
                    }
                break;
            }
        }

    });

    ws.on("close", (client, reason) => {
        console.log("Client closed");
    });

    let key = "";
    for(var place=0 ; place < saltKey.length ; place++) {
        key += String.fromCharCode(saltKey.charCodeAt(place)*2);
    }

    ws.send(key);

});
