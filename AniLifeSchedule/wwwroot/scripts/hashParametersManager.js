export function checkHash() {
    return window.location.hash !== "";
}

export function getAuthHashParameters() {
    const hash = window.location.hash.substring(1);
    const params = new URLSearchParams(hash);
    return {
        access_token: params.get('access_token'),
        expires_in: params.get('expires_in'),
        user_id: params.get('user_id')
    };
}