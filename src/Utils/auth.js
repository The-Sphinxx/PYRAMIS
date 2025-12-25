// Shared JWT helpers
// Keeps token decoding logic centralized so components and stores stay lean.

const base64UrlDecode = (segment) => {
  try {
    const padded = segment.replace(/-/g, '+').replace(/_/g, '/').padEnd(Math.ceil(segment.length / 4) * 4, '=');
    const decoded = atob(padded);
    return decodeURIComponent(
      decoded
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
  } catch (error) {
    console.error('JWT decode failed', error);
    return null;
  }
};

export const decodeJwt = (token) => {
  if (!token || typeof token !== 'string' || !token.includes('.')) return null;
  const [, payload] = token.split('.');
  const json = base64UrlDecode(payload);
  if (!json) return null;
  try {
    return JSON.parse(json);
  } catch (error) {
    console.error('JWT payload parse failed', error);
    return null;
  }
};

const claim = (decoded, keys) => {
  for (const key of keys) {
    if (decoded[key]) return decoded[key];
  }
  return '';
};

export const buildUserFromToken = (token) => {
  const decoded = decodeJwt(token);
  if (!decoded) return null;

  const firstName = claim(decoded, ['given_name', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']);
  const lastName = claim(decoded, ['family_name', 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']);
  const fullName = claim(decoded, ['name', 'fullName']) || [firstName, lastName].filter(Boolean).join(' ');

  return {
    id:
      claim(decoded, [
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
        'nameid',
        'sub'
      ]) || null,
    email: claim(decoded, ['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress', 'email']) || null,
    firstName,
    lastName,
    fullName,
    role: claim(decoded, ['http://schemas.microsoft.com/ws/2008/06/identity/claims/role', 'role']) || '',
    profileImage: claim(decoded, ['avatar', 'profileImage', 'picture']) || '',
    membershipType: claim(decoded, ['membershipType', 'membership']) || ''
  };
};

export const getUserFromToken = (providedToken) => {
  const token = providedToken || localStorage.getItem('token');
  if (!token) return null;
  return buildUserFromToken(token);
};
