import React, { Fragment, useEffect } from "react";
import {
  BrowserRouter,
  HashRouter,
  Navigate,
  Routes,
  Route,
} from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { LOGIN_REFRESH } from "../../redux/constants/authConstants";

const ProtectedRoute = ({ Component, ...rest }) => {
  const dispatch = useDispatch();

  const { loading, isAuthenticated, user, error } = useSelector(
    (state) => state.auth
  );

  useEffect(() => {
    const access_token = window.localStorage.getItem("access_token");
    const expires_at = window.localStorage.getItem("expires_at");
    if (!isAuthenticated) {
      if (access_token && expires_at) {
        dispatch({ type: LOGIN_REFRESH });
      }
    }
  }, [dispatch]);

  return isAuthenticated ? <Component /> : <Navigate to="/login" />;

  //   return (
  //     <Fragment>
  //       {loading === false && (
  //         <Route
  //           {...rest}
  //           render={(props) => {
  //             if (isAuthenticated === false) {
  //               return <Navigate to="/login" />;
  //             }

  //             // if (isAdmin === true && user.role !== "admin") {
  //             //   return <Navigate to="/" />;
  //             // }

  //             return <Component {...props} />;
  //           }}
  //         />
  //       )}
  //     </Fragment>
  //   );
};

export default ProtectedRoute;
