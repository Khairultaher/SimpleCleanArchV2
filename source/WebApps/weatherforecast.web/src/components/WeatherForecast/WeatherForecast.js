import React, { Component, Fragment, useState, useEffect } from "react";
import Pagination from "react-js-pagination";
import Swal from "sweetalert2";
import AddWeatherForecast from "./AddWeatherForecast";
import { format } from "date-fns";

import { useDispatch, useSelector } from "react-redux";
import { useAlert } from "react-alert";
import useAuth from "../../hooks/useAuth";
import {
  getAllForcast,
  deleteForcast,
  clearErrors,
} from "../../redux/actions/weatherForcastActions";

import Loader from "../layout/Loader";

const WeatherForecast = ({}) => {
  const [currentPage, setCurrentPage] = useState(1);
  const [itemPerPage, setItemPerPage] = useState(10);
  const [showModal, setShowModal] = useState(false);
  const [data, setData] = useState({ id: null, temperatureC: 0, summary: "" });
  const [onDeleteConfirmation, setOnDeleteConfirmation] = useState(false);
  const [itemToDelete, setItemToDelete] = useState(null);

  const alert = useAlert();
  const dispatch = useDispatch();
  const { loading, forecasts, success, error, totalCount } = useSelector(
    (state) => state.allForcast
  );
  const { auth, setAuth } = useAuth();

  useEffect(() => {
    if (error) {
      alert.error(error);
      dispatch(clearErrors());
    }

    dispatch(getAllForcast(currentPage, itemPerPage, auth.token.accessToken));
  }, [dispatch, currentPage, alert, error]);

  function setCurrentPageNo(pageNumber) {
    setCurrentPage(pageNumber);
  }
  function showModalDialog(param) {
    setData(param);
    setShowModal(true);
  }
  function onCloseModal() {
    setShowModal(!showModal);

    dispatch(getAllForcast(currentPage, itemPerPage, auth.token.accessToken));
  }
  function onDeleteRequest(id) {
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        onDeleteConfirm(id);
      }
    });
  }
  async function onDeleteConfirm(id) {
    await dispatch(deleteForcast(id));

    if (success) {
      Swal.fire("Deleted!", "Your file has been deleted.", "success");
    }
    dispatch(getAllForcast(currentPage, itemPerPage));
  }
  function onDeleteCancel() {
    setItemToDelete(null);
    setOnDeleteConfirmation(false);
  }
  function onSave() {}
  return (
    <>
      <section className="container-fluid">
        <div className="content-header">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1 className="m-0 text-dark">Dashboard</h1>
            </div>
            <div className="col-sm-6">
              <ol className="breadcrumb float-sm-right">
                <li className="breadcrumb-item">
                  <a href="#">Home</a>
                </li>
                <li className="breadcrumb-item active">Dashboard v1</li>
              </ol>
            </div>
          </div>
        </div>
      </section>
      <section className="container-fluid">
        <div className="content">
          <Fragment>
            {loading ? (
              <Loader />
            ) : (
              <Fragment>
                <div className="row float-right">
                  <button
                    type="button"
                    data-toggle="modal"
                    data-target="#myModal"
                    className="btn btn-success btn-sm my-1 mx-2 px-3 float-right"
                    onClick={() => {
                      showModalDialog(data);
                    }}
                  >
                    <i className="fas fa-plus"></i> Add
                  </button>
                </div>
                <br></br>

                <section id="content">
                  <table
                    className="table table-striped"
                    aria-labelledby="tabelLabel"
                  >
                    <thead>
                      <tr>
                        <th>Date</th>
                        <th>Location</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                        <th>Actios</th>
                      </tr>
                    </thead>
                    <tbody>
                      {forecasts.map((forecast) => (
                        <tr key={forecast.id}>
                          <td>
                            {format(new Date(forecast.date), "yyyy-MM-dd")}
                          </td>
                          <td>{forecast.location}</td>
                          <td>{forecast.temperatureC}</td>
                          <td>{forecast.temperatureF}</td>
                          <td>{forecast.summary}</td>
                          <td>
                            <div className="d-flex justify-content-start">
                              <a
                                className="btn mr-1"
                                onClick={() => {
                                  showModalDialog({
                                    id: forecast.id,
                                    temperatureC: forecast.temperatureC,
                                    location: forecast.location,
                                    summary: forecast.summary,
                                  });
                                }}
                              >
                                <i className="fas fa-edit text-success"></i>
                              </a>
                              <a
                                className="btn"
                                onClick={() => {
                                  onDeleteRequest(forecast.id);
                                }}
                              >
                                <i className="fas fa-trash-alt text-danger"></i>
                              </a>
                            </div>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </section>
                <section id="paggination">
                  {itemPerPage <= totalCount && (
                    <div className="d-flex justify-content-center mt-5">
                      <Pagination
                        activePage={currentPage}
                        itemsCountPerPage={itemPerPage}
                        totalItemsCount={totalCount}
                        onChange={setCurrentPageNo}
                        nextPageText={"Next"}
                        prevPageText={"Prev"}
                        firstPageText={"First"}
                        lastPageText={"Last"}
                        itemClass="page-item"
                        linkClass="page-link"
                      />
                    </div>
                  )}
                </section>
                <section id="modal">
                  {
                    <AddWeatherForecast
                      show={showModal}
                      data={data}
                      onSave={onSave}
                      onCloseModalClick={onCloseModal}
                    ></AddWeatherForecast>
                  }
                </section>
              </Fragment>
            )}
          </Fragment>
        </div>
      </section>
    </>
  );
};

export default WeatherForecast;
