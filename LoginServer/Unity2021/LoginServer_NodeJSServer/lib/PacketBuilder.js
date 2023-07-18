
let packetbuilder = {}

packetbuilder.create = function() {
    /*
    
    let size = 7234;
    var arrbuff = new ArrayBuffer(4);
    var dview = new DataView(arrbuff);
    dview.setInt32(0, size, false);
    console.log(dview);
    console.log(new Int8Array(arrbuff)[0]);
    console.log(new Int8Array(arrbuff)[1]);
    console.log(new Int8Array(arrbuff)[2]);
    console.log(new Int8Array(arrbuff)[3]);
    */

    let ret = {}
    ret.packet = [];

    ret.set = function(bytedata) {
        console.log(`new bytedata: ${bytedata}`);
        while(bytedata[0]!=undefined) {
            ret.packet.push(bytedata[0]);
        }
        console.log(ret.packet);
    }

    ret.inverse = function() {
        let oldpacket = ret.packet;
        let newpacket = [];
        while(oldpacket > 0) {
            let val = oldpacket.pop();
            console.log(`val: ${val}`);
            newpacket.push(val);
        }
        ret.packet = newpacket;
    }

    ret.WriteInt = function(val) {
        var arrbuff = new ArrayBuffer(4);
        var dview = new DataView(arrbuff);
        dview.setInt32(0, val, false);
        var intArray = new Int8Array(arrbuff);
        ret.packet.push(intArray[0]);
        ret.packet.push(intArray[1]);
        ret.packet.push(intArray[2]);
        ret.packet.push(intArray[3]);
    }

    return ret;
}

module.exports = packetbuilder;