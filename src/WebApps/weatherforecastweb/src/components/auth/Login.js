import React, {
  Component,
  Fragment,
  useState,
  useEffect,
  useRef,
  useContext,
} from "react";

import { useDispatch, useSelector } from "react-redux";
import { useAlert } from "react-alert";
import { Link, useLocation, useNavigate } from "react-router-dom";
import Loader from "../layout/Loader";
import MetaData from "../layout/MetaData";
import { login, clearErrors } from "../../redux/actions/authActions";
import useAuth from "../../hooks/useAuth";

const Login = ({}) => {
  const { setAuth, persist, setPersist } = useAuth();

  const [userLoginName, setUserLoginName] = useState();
  const [password, setPassword] = useState();

  const alert = useAlert();
  const dispatch = useDispatch();
  const { loading, isAuthenticated, userName, roles, claims, token, error } =
    useSelector((state) => state.auth);

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  useEffect(() => {
    if (error) {
      alert.error(error);
      dispatch(clearErrors());
    }

    if (isAuthenticated) {
      setAuth({ isAuthenticated, userName, roles, claims, token });
      localStorage.setItem("persist", true);
      navigate(from, { replace: true });
    }
  }, [isAuthenticated]);

  const submitHandler = async (e) => {
    e.preventDefault();

    if (!isAuthenticated) {
      await dispatch(login(userLoginName, password));
    }
  };

  // const togglePersist = () => {
  //   setPersist((prev) => !prev);
  // };

  // useEffect(() => {
  //   localStorage.setItem("persist", persist);
  // }, [persist]);

  return (
    <Fragment>
      {loading ? (
        <div className="vh-100 d-flex justify-content-center align-items-center">
          <div className="col-10 col-lg-5">
            <div className="mb-5">
              <Loader />
            </div>
          </div>
        </div>
      ) : (
        <Fragment>
          <MetaData title={"Login"} />
          <div className="vh-100 d-flex justify-content-center align-items-center">
            <div className="shadow-lg col-10 col-lg-5">
              <form className="px-3 py-3" onSubmit={submitHandler}>
                <h1 className="mb-3">Login</h1>
                <div className="form-group">
                  <label htmlFor="email_field">User Name</label>
                  <input
                    type="text"
                    id="userName"
                    className="form-control"
                    value={userLoginName}
                    onChange={(e) => setUserLoginName(e.target.value)}
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="password_field">Password</label>
                  <input
                    type="password"
                    id="password"
                    className="form-control"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                  />
                </div>

                <Link to="/password/forgot" className="float-right mb-4">
                  Forgot Password?
                </Link>

                <button
                  id="login_button"
                  type="submit"
                  className="btn btn-block btn-success"
                >
                  LOGIN
                </button>

                <div className="form-group">
                  <Link to="/register" className="float-right mt-3 mb-3">
                    New User?
                  </Link>
                </div>
              </form>
            </div>
          </div>
        </Fragment>
      )}
    </Fragment>
  );
};

export default Login;
