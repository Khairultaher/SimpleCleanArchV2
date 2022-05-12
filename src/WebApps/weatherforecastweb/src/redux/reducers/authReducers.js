import {
  LOGIN_REQUEST,
  LOGIN_REFRESH,
  LOGIN_SUCCESS,
  LOGIN_FAIL,
  LOGOUT_REQUEST,
  LOGOUT_SUCCESS,
  LOGOUT_FAIL,
  CLEAR_ERRORS,
} from "../constants/authConstants";

export const authReducer = (state = { user: {} }, action) => {
  switch (action.type) {
    case LOGIN_REQUEST:
      return {
        ...state,
        isAuthenticated: false,
        userName: null,
        token: null,
        roles: null,
        claims: null,
        loading: true,
      };
    case LOGIN_REFRESH:
      return {
        ...state,
        isAuthenticated: true,
        loading: true,
      };
    case LOGIN_SUCCESS:
      return {
        ...state,
        isAuthenticated: true,
        userName: action.payload.userName,
        token: action.payload.token,
        roles: action.payload.roles,
        claims: action.payload.claims,
        loading: false,
      };
    case LOGIN_FAIL:
      return {
        ...state,
        isAuthenticated: false,
        userName: null,
        token: null,
        roles: null,
        claims: null,
        loading: false,
      };
    case LOGOUT_REQUEST:
      return {
        ...state,
        loading: true,
      };
    case LOGOUT_SUCCESS:
      return {
        ...state,
        loading: false,
        isAuthenticated: false,
        userName: null,
        token: null,
        roles: null,
        claims: null,
      };
    case LOGOUT_FAIL:
      return {
        ...state,
        loading: false,
      };
    case CLEAR_ERRORS:
      return {
        ...state,
        loading: false,
        error: null,
      };
    default:
      return {
        ...state,
        loading: false,
        isAuthenticated: false,
        userName: null,
        token: null,
        roles: null,
        claims: null,
        error: null,
      };
  }
};
