import { CredentialManager, XRPC } from '@atcute/client'

export const serviceEndpoint = 'http://localhost:5084'

const manager = new CredentialManager({
  service: serviceEndpoint,
})
export const xrpc = new XRPC({ handler: manager })
