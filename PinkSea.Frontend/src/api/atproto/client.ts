import { CredentialManager, XRPC } from '@atcute/client'

console.log(import.meta.env);
export const serviceEndpoint = window.pinkSeaConfig?.apiUrl || import.meta.env.PINKSEA_API_URL;

const manager = new CredentialManager({
  service: serviceEndpoint,
})
export const xrpc = new XRPC({ handler: manager })
