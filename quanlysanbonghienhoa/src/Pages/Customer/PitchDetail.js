import { useParams } from "react-router-dom";
import "../Customer/CustomerSchedule";
import CustomerSchedule from "../Customer/CustomerSchedule";
const PitchDetail = () => {
  const { pitchId } = useParams();

  return (
    <div>
      <h2>Chi tiết sân bóng</h2>
      <CustomerSchedule pitchId={pitchId} />
    </div>
  );
}

export default PitchDetail;