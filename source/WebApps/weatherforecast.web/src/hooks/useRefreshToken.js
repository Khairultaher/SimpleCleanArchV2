import axios from "axios";
import useAuth from "./useAuth";
import config from "../config.json";

const useRefreshToken = () => {
  const { auth, setAuth } = useAuth();
  const link = `${config.API_BASE_URL}/api/token/refresh`;
  const token = JSON.parse(localStorage.getItem("token"));
  const refresh = async () => {
    try {
      const config = {
        headers: {
          "Content-Type": "application/json",
        },
      };

      const res = await axios.post(link, token, config);
      if (res.status === 200) {
        setAuth((prev) => {
          const newAuth = { ...prev };
          newAuth.token = res.data.token;
          return { ...newAuth };
        });
        localStorage.setItem("token", JSON.stringify(res.data.token));
        return res.data.token;
      } else {
        localStorage.setItem("token", JSON.stringify(auth.token));
        return auth.token;
      }
    } catch (err) {
      console.log(err.message);
    }
  };
  return refresh;
};

export default useRefreshToken;
