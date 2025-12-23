/// <reference types="vite/client" />
interface ImportMetaEnv {
  readonly VITE_APP_BASE_URL: string;
  readonly VITE_APP_DEFAULT_LANGUAGE: string;
  readonly VITE_APP_ENABLE_MULTI_LANGUAGE: string;
  readonly VITE_APP_CDN_URL: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
