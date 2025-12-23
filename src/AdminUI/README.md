## Techical stuffs

[Vue 3](https://vuejs.org/guide/introduction.html) 
with [Vite](https://vitejs.dev/guide/)
and [Primevue](https://primevue.org/)<br>
[Pinia](https://pinia.vuejs.org/)<br>
[Vuelidate](https://vuelidate.js.org/)<br>
[vue-i18n](https://vue-i18n.intlify.dev/)<br>
[SheetJS](https://sheetjs.com/)


## LOCAL setup

**1/** Restore packages: 
> yarn

or

> npm install

**2/** Start the application
> yarn run dev

or

> npm run dev

## Configuration files:
**.env.dev**: for dev environment <br>
**.env.production**: for production environment

## Global Event Listener
**1/** On the parent or any component needs to listen, add listener

    created(): void {
      emitter.on("update", (value) => console.log("RECEIVED!", value));
    }

**2/** Add the emit action on code:

    import { emitter } from "@/plugins/Emitter";
    emitter.emit("update", "Hello");

## Add localization to components
**1/** Add keys for translation:
Currently, we are having 2 language files (*en-US.json*, *vi-VN.json*) set up and located in
  src/locales/<language_code>.json
We have to put the key there with the format as below
  - In user form, if user has field "name" we will add key "User.Create.Label.Name". Format <component_name>.<action>.<messsage_position>.<message_content> (Optional <message_content> "User.Create.Title")
  - For enum, we could use <enum_name>.<enum_value> for commonly using. Eg: "UserStatus.Active"
  
**2/** Using in html: 

    {{ $t("<translation_key>") }}

**3/** Using in code:

    this.$t("<translation_key>")

**4/** Using formating:

    Text: '{msg} world'
    Implement: {{ $t('message.hello', { msg: 'hello' }) }}

    Text: '{0} world'
    Implement: {{ $t('message.hello', ['hello']) }}
    
## Setup .prettier
1/ Open workspace settings (JSON) by pressing Windows + Shift + P
2/ Update content as
{
  "eslint.validate": ["vue", "html", "javascript", "typescript"],
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": "explicit"
  },
  "vetur.format.defaultFormatter.html": "js-beautify-html",
  "[vue]": {
    "editor.defaultFormatter": "rvest.vs-code-prettier-eslint"
  },
  "[scss]": {
    "editor.defaultFormatter": "rvest.vs-code-prettier-eslint",
    "editor.formatOnSave": true
  },
  "editor.formatOnSave": true,
  "typescript.preferences.autoImportFileExcludePatterns": ["vue-router"],
  "vetur.format.defaultFormatterOptions": {
    "prettier": {
      "singleQuote": false
    }
  },
  "git.ignoreLimitWarning": true
}

