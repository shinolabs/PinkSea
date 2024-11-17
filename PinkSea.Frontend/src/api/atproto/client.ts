import { CredentialManager, XRPC } from '@atcute/client'

export const serviceEndpoint = 'https://api.pinksea.art'

const manager = new CredentialManager({
  service: serviceEndpoint,
})
export const xrpc = new XRPC({ handler: manager })
