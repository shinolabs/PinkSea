import { CredentialManager, XRPC } from '@atcute/client'

const manager = new CredentialManager({
  service: "http://localhost:5084"
});
export const xrpc = new XRPC({ handler: manager });
