const fs = require('fs');
const path = require('path');

const indexPath = path.join(__dirname, 'auto/autolmsclient-abstractions/index.ts');

function addOverrideKeyword() {
    fs.readFile(indexPath, 'utf8', (err, data) => {
        if (err) {
            console.error('Error reading index.ts:', err);
            return;
        }

        const messageRegex = /(\s*message:\s*string\s*;)/;
        const overrideMessageRegex = /(\s*override\s+message:\s*string\s*;)/;

        if (!overrideMessageRegex.test(data) && messageRegex.test(data)) {
            const updatedData = data.replace(messageRegex, '\n\toverride message: string;');
            fs.writeFile(indexPath, updatedData, 'utf8', (err) => {
                if (err) {
                    console.error('Error writing to index.ts:', err);
                } else {
                    console.log('Successfully added override keyword to message: string');
                }
            });
        } else {
            console.log('Override keyword already present or message: string not found');
        }
    });
}

addOverrideKeyword();