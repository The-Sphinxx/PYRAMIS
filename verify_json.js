const fs = require('fs');
try {
    const content = fs.readFileSync('db.json', 'utf8');
    JSON.parse(content);
    console.log('JSON is valid');
} catch (e) {
    console.log(e.message);
}
