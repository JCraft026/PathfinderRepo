const fs = require('fs');

let badlist = fs.readFileSync('./badlist.txt', 'utf8').split('\n');

badlist = badlist.map(word => (".*" + word + ".*")).join('\n');

fs.writeFileSync('./badlist.txt', badlist);
/*
*/

