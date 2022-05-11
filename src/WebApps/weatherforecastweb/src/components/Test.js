import React, { useEffect, useState } from "react";
import useRefreshToken from "../hooks/useRefreshToken";

const Test = () => {
  const refresh = useRefreshToken();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    //console.log(mounted);
  }, [mounted]);

  return (
    <section>
      <h1>Test</h1>
      <br />
      <p>Refresh your access token here.</p>
      <div className="flexGrow">
        <button
          onClick={async () => {
            let res = await refresh();
            setMounted(false);
          }}
        >
          Refresh
        </button>
      </div>
    </section>
  );
};

export default Test;
