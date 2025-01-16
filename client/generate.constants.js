const fs = require('fs').promises;
const path = require('path');
const xml2js = require('xml2js');

const resxFilePath = path.join(__dirname, 'src/resources/resources.resx');
const outputFilePath = path.join(__dirname, 'src/constants/constants.ts');

const parser = new xml2js.Parser();

async function readResxFile(filePath) {
  try {
    return await fs.readFile(filePath, 'utf8');
  } catch (error) {
    throw new Error(`Error reading resx file: ${error.message}`);
  }
}

async function parseResxData(data) {
  return new Promise((resolve, reject) => {
    parser.parseString(data, (err, result) => {
      if (err) {
        reject(new Error(`Error parsing resx file: ${err.message}`));
      } else {
        resolve(result);
      }
    });
  });
}

function generateConstants(result) {
  return result.root.data.reduce((acc, item) => {
    const key = item.$.name.replace('display_text_', '')
      .split('_')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join('');
    const value = item.value[0];
    acc[key] = { name: item.$.name, value };
    return acc;
  }, {});
}

function createConstantsContent(constants) {
  return `export enum DisplayConstants {
${Object.entries(constants).map(([key, { name, value }]) => `  /** @constant ${name} */\n  ${key} = "${value}",`).join('\n\n')}
}`;
}

async function writeConstantsFile(filePath, content) {
  try {
    await fs.writeFile(filePath, content, 'utf8');
    console.log('Constants file generated successfully.');
  } catch (error) {
    throw new Error(`Error writing constants file: ${error.message}`);
  }
}

async function main() {
  try {
    const data = await readResxFile(resxFilePath);
    const result = await parseResxData(data);
    const constants = generateConstants(result);
    const constantsContent = createConstantsContent(constants);
    await writeConstantsFile(outputFilePath, constantsContent);
  } catch (error) {
    console.error(error.message);
  }
}

main();