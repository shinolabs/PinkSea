import { CredentialManager, XRPC } from '@atcute/client'

export const serviceEndpoint = window.pinkSeaConfig?.apiUrl || import.meta.env.VITE_PINKSEA_API_URL;

const manager = new CredentialManager({
  service: serviceEndpoint,
})
export const xrpc = new XRPC({ handler: manager })
