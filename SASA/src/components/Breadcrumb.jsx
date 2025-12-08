import React from "react";

function Breadcrumb({ items }) {
  const trail =
    items && items.length
      ? items
      : [{ label: "Inicio" }];

  return (
    <div className="mb-4">
      <nav className="text-[11px] text-[#BFBFBF]">
        <ol className="flex flex-wrap items-center gap-1">
          {trail.map((item, index) => {
            const isLast = index === trail.length - 1;

            return (
              <li key={index} className="flex items-center gap-1">
                <span
                  className={
                    isLast
                      ? "font-semibold text-[#DBDBDB]"
                      : "text-[#BFBFBF]"
                  }
                >
                  {item.label}
                </span>
                {!isLast && (
                  <span className="text-[#BFBFBF]">/</span>
                )}
              </li>
            );
          })}
        </ol>
      </nav>
    </div>
  );
}

export default Breadcrumb;
