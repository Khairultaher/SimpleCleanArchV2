import axios from "axios";
import useAuth from "./useAuth";
import config from "../config.json";

const useLogout = () => {
  const { auth, setAuth } = useAuth();
  const link = `${config.API_BASE_URL}/api/auth/logout`;

  const { userName, token } = { ...auth };

  const logout = async () => {
    setAuth({
      isAuthenticated: false,
      userName: null,
      roles: null,
      claims: null,
      token: null,
    });
    try {
      const config = {
        headers: {
          "Content-Type": "application/json",
        },
      };
      const response = await axios.post(link, userName, config);
      localStorage.removeItem("token");
    } catch (err) {
      console.error(err);
    }
  };

  return logout;
};

export default useLogout;
