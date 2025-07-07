export default interface Profile {
    did: string,
    handle: string,
    nick?: string,
    description?: string,
    links?: {
        name: string,
        url: string
    }[],
    avatar?: string
}